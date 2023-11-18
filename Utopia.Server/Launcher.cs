// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Globalization;
using Autofac;
using CommunityToolkit.Diagnostics;
using Npgsql;
using Utopia.Core;
using Utopia.Core.Collections;
using Utopia.Core.Events;
using Utopia.Core.Logging;
using Utopia.Core.Plugin;
using Utopia.Core.Transition;
using Utopia.Core.Utilities;
using Utopia.Core.Utilities.IO;
using Utopia.Server.Entity;
using Utopia.Server.Logic;
using Utopia.Server.Map;
using Utopia.Server.Net;

namespace Utopia.Server;

/// <summary>
/// 服务器启动器
/// </summary>
public static class Launcher
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    public static void WaitForStart(object locker)
    {
        ArgumentNullException.ThrowIfNull(locker);
        _ = Thread.Yield();
        Thread.Sleep(100);
        lock (locker)
        {
            return;
        }
    }

    /// <summary>
    /// 启动参数
    /// </summary>
    public class LauncherOption
    {
        /// <summary>
        /// 服务器端口
        /// </summary>
        public int Port { get; set; } = 1145;

        /// <summary>
        /// 是否跳过初始化log系统
        /// </summary>
        public bool SkipInitLog { get; set; } = false;

        public LogManager.LogOption LogOption { get; set; } = LogManager.LogOption.CreateDefault();

        /// <summary>
        /// 文件系统
        /// </summary>
        public IFileSystem? FileSystem { get; set; } = null;

        /// <summary>
        /// 数据库链接，must not be null
        /// </summary>
        public NpgsqlDataSource DatabaseSource { get; set; } = null!;

        /// <summary>
        /// 用于设置全局语言标识符
        /// </summary>
        public TranslateIdentifence GlobalLanguage { get; set; } =
            new(CultureInfo.CurrentCulture.ThreeLetterISOLanguageName,
            RegionInfo.CurrentRegion.ThreeLetterISORegionName);
    }

    /// <summary>
    /// 使用字符串参数启动服务器
    /// </summary>
    /// <param name="args">命令行参数</param>
    public static void LaunchWithArguments(string[] args, object? locker)
    {
        ArgumentNullException.ThrowIfNull(args, nameof(args));

        var option = new LauncherOption();

        int i = 0;
        while (i != args.Length)
        {
            string arg = args[i++];

            if (arg == "--port")
            {
                if (i == args.LongLength)
                {
                    throw new ArgumentException("--port argument need one number");
                }
                option.Port = int.Parse(args[i++]);
            }
            else if (arg == "--skip-log-init")
            {
                option.SkipInitLog = true;
            }
            else if (arg == "--batch")
            {
                option.LogOption = LogManager.LogOption.CreateBatch();
            }
            else if (arg == "--postgreSql")
            {
                if (i == args.LongLength)
                {
                    throw new ArgumentException("--port argument need one number");
                }
                var dataSourceBuilder = new NpgsqlDataSourceBuilder(args[i++]);
                _ = dataSourceBuilder.UseLoggerFactory(new
                    NLog.Extensions.Logging.NLogLoggerFactory());
                NpgsqlDataSource dataSource = dataSourceBuilder.Build();
                option.DatabaseSource = dataSource;
            }
            else
            {
                throw new ArgumentException("unknown command line argument:" + arg);
            }
        }

        Launch(option, locker);
    }

    /// <summary>
    /// 使用参数启动服务器
    /// </summary>
    /// <param name="option">参数</param>
    public static void Launch(LauncherOption option, object? locker)
    {
        locker ??= new object();
        lock (locker)
        {
            ArgumentNullException.ThrowIfNull(option);

            Thread.CurrentThread.Name = "Server Initialization Thread";

            ServiceProvider provider = _InitSystem(option);

            IEventBus bus = provider.GetService<IEventBus>();

            try
            {
                _ = provider.TryRegisterService(LifeCycle.InitializedSystem);
                LifeCycleEvent<LifeCycle>.EnterCycle(LifeCycle.InitializedSystem, () =>
                {
                    // do nothing
                }, s_logger, bus, provider);

                // 加载插件
                LifeCycleEvent<LifeCycle>.EnterCycle(LifeCycle.LoadPlugin, () =>
                {
                    provider.GetService<IPluginLoader<IPlugin>>().Register(
                            provider.GetService<ContainerBuilder>(),
                            typeof(Plugin.Plugin)
                        );
                    provider.GetService<IPluginLoader<IPlugin>>().LoadFromDirectory(
                        provider.GetService<IFileSystem>().PluginsDirectory,
                        provider.GetService<ContainerBuilder>(),
                        s_logger
                    );
                    IContainer container = provider.GetService<ContainerBuilder>().Build();
                    provider.RemoveService<ContainerBuilder>();
                    _ = provider.TryRegisterService(container);
                    provider.GetService<IPluginLoader<IPlugin>>().Active(container);

                }, s_logger, bus, provider);

                // 创建世界
                LifeCycleEvent<LifeCycle>.EnterCycle(LifeCycle.LoadSavings, () => { _LoadSave(provider); }, s_logger, bus, provider);

                // 设置逻辑线程
                LifeCycleEvent<LifeCycle>.EnterCycle(LifeCycle.StartLogicThread, () => { _StartLogicThread(provider); }, s_logger, bus, provider);

                // 设置网络线程
                LifeCycleEvent<LifeCycle>.EnterCycle(LifeCycle.StartNetThread, () => { _StartNetworkThread(provider, option.Port); }, s_logger, bus, provider);
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "the server initlize failed");
                LifeCycleEvent<LifeCycle>.EnterCycle(LifeCycle.Crash, () => { }, s_logger, bus, provider);
                LifeCycleEvent<LifeCycle>.EnterCycle(LifeCycle.Stop, () => { }, s_logger, bus, provider);
            }
        }
    }

    /// <summary>
    /// 初始化系统
    /// </summary>
    private static ServiceProvider _InitSystem(LauncherOption option)
    {
        Guard.IsNotNull(option);
        // 初始化日志系统
        if (!option.SkipInitLog)
        {
            LogManager.Init(option.LogOption);
        }

        // 初始化依赖注入和服务提供者
        ServiceProvider provider = new();
        ContainerBuilder builder = new();

        register<IFileSystem>(option.FileSystem ?? new FileSystem());
        register<Microsoft.Extensions.Logging.ILoggerFactory>(new NLog.Extensions.Logging.NLogLoggerFactory());
        register<Core.IServiceProvider>(provider);
        register<IPluginLoader<IPlugin>>(new PluginLoader<IPlugin>());
        register<ITranslateManager>(new TranslateManager());
        register<TranslateIdentifence>(option.GlobalLanguage);
        register<ILogicThread>(new StandardLogicThread());
        register<IEventBus>(new EventBus());
        register<IEntityManager>(new EntityManager());
        register<IInternetMain>(new InternetMain(provider));
        // init filesystem
        provider.GetService<IFileSystem>().CreateIfNotExist();

        // as world manager
        register<SafeDictionary<long, IWorld>>(new SafeDictionary<long, Map.IWorld>());
        register<SafeDictionary<Guuid, IWorldFactory>>(new SafeDictionary<Guuid, IWorldFactory>());

        _ = provider.TryRegisterService(builder);

        if (option.DatabaseSource == null)
        {
            // throw new ArgumentException("Database Source option need to be set");
        }
        // TODO:ADD PGSQL

        return provider;

        void register<T>(object instance) where T : notnull
        {
            _ = provider.TryRegisterService<T>((T)instance);
            _ = builder.RegisterInstance(instance).As<T>().ExternallyOwned();
        }
    }

    /// <summary>
    /// 加载存档
    /// </summary>
    /// <param name="provider"></param>
    private static void _LoadSave(Core.IServiceProvider provider)
    {
        KeyValuePair<Guuid, IWorldFactory>[] array = provider.GetService<SafeDictionary<Guuid, IWorldFactory>>().ToArray();

        // TODO: Load saves from file
        if (array.Length == 1)
        {
            _ = provider.GetService<SafeDictionary<long, IWorld>>().TryAdd(0, array[0].Value.GenerateNewWorld());
        }
    }

    /// <summary>
    /// 启动
    /// </summary>
    /// <param name="provider"></param>
    private static void _StartLogicThread(Core.IServiceProvider provider)
    {
        var logicT = new Thread(() =>
        {
            ILogicThread t = provider.GetService<ILogicThread>();
            KeyValuePair<long, IWorld>[] worlds = provider.GetService<SafeDictionary<long, IWorld>>().ToArray();
            foreach (KeyValuePair<long, IWorld> world in worlds)
            {
                t.Add(world.Value);
            }
            // 注册关闭事件
            provider.GetService<IEventBus>().Register<LifeCycleEvent<LifeCycle>>((cycle) =>
            {
                if (cycle.Cycle == LifeCycle.Stop && cycle.Order == LifeCycleOrder.After)
                {
                    t.StopTokenSource.Cancel();
                }
            });
            ((StandardLogicThread)t).Run();
        })
        {
            Name = "Server Logic Thread"
        };
        logicT.Start();
    }

    /// <summary>
    /// 启动网络线程
    /// </summary>
    private static void _StartNetworkThread(Core.IServiceProvider provider, int port)
    {
        IInternetListener netServer = provider.GetService<IInternetListener>();
        _ = provider.TryRegisterService(netServer);
        netServer.Listen(port);
        s_logger.Info("listen to {port}", port);

        InternetMain netThread = (InternetMain)provider.GetService<IInternetMain>();
        var thread = new Thread(() =>
        {
            // 注册关闭事件
            provider.GetService<IEventBus>().Register<LifeCycleEvent<LifeCycle>>((cycle) =>
            {
                if (cycle.Cycle == LifeCycle.Stop && cycle.Order == LifeCycleOrder.After)
                {
                    netThread.StopTokenSource.Cancel();
                }
            });
            netThread.Run();
        })
        {
            Name = "Server Networking Thread"
        };
        thread.Start();
    }

    private static void Main(string[] args) => LaunchWithArguments(args, null);
}
