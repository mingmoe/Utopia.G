#region copyright
// This file(may named Launcher.cs) is a part of the project: Utopia.Server.
// 
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// 
// This file is part of Utopia.Server.
//
// Utopia.Server is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Utopia.Server is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with Utopia.Server. If not, see <https://www.gnu.org/licenses/>.
#endregion

using Autofac;
using CommunityToolkit.Diagnostics;
using Npgsql;
using System.Globalization;
using Utopia.Core;
using Utopia.Core.Collections;
using Utopia.Core.Events;
using Utopia.Core.Logging;
using Utopia.Core.Translate;
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
    private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

    public static void WaitForStart(object locker)
    {
        ArgumentNullException.ThrowIfNull(locker);
        Thread.Yield();
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
            var arg = args[i++];

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
                dataSourceBuilder.UseLoggerFactory(new
                    NLog.Extensions.Logging.NLogLoggerFactory());
                var dataSource = dataSourceBuilder.Build();
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

            var provider = _InitSystem(option);

            var bus = provider.GetService<IEventBus>();

            try
            {
                provider.TryRegisterService(LifeCycle.InitializedSystem);
                LifeCycleEvent<LifeCycle>.EnterCycle(LifeCycle.InitializedSystem, () =>
                {
                    // do nothing
                }, _logger, bus, provider);

                // 加载插件
                LifeCycleEvent<LifeCycle>.EnterCycle(LifeCycle.LoadPlugin, () =>
                {
                    provider.GetService<IPluginLoader<IPlugin>>().Register(
                            provider.GetService<ContainerBuilder>(),
                            typeof(Plugin.CorePlugin)
                        );
                    provider.GetService<IPluginLoader<IPlugin>>().LoadFromDirectory(
                        provider.GetService<IFileSystem>().Plugins,
                        provider.GetService<ContainerBuilder>(),
                        _logger
                    );
                    var container = provider.GetService<ContainerBuilder>().Build();
                    provider.RemoveService<ContainerBuilder>();
                    provider.TryRegisterService(container);
                    provider.GetService<IPluginLoader<IPlugin>>().Active(container);

                }, _logger, bus, provider);

                // 创建世界
                LifeCycleEvent<LifeCycle>.EnterCycle(LifeCycle.LoadSaveings, () => { _LoadSave(provider); }, _logger, bus, provider);

                // 设置逻辑线程
                LifeCycleEvent<LifeCycle>.EnterCycle(LifeCycle.StartLogicThread, () => { _StartLogicThread(provider); }, _logger, bus, provider);

                // 设置网络线程
                LifeCycleEvent<LifeCycle>.EnterCycle(LifeCycle.StartNetThread, () => { _StartNetworkThread(provider, option.Port); }, _logger, bus, provider);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "the server initlize failed");
                LifeCycleEvent<LifeCycle>.EnterCycle(LifeCycle.Crash, () => { }, _logger, bus, provider);
                LifeCycleEvent<LifeCycle>.EnterCycle(LifeCycle.Stop, () => { }, _logger, bus, provider);
            }
        }
    }

    /// <summary>
    /// 初始化系统
    /// </summary>
    static ServiceProvider _InitSystem(LauncherOption option)
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
        register<ILogicThread>(new SimplyLogicThread());
        register<IEventBus>(new EventBus());
        register<IEntityManager>(new EntityManager());
        register<InternetMain>(new InternetMain(provider));
        // init filesystem
        provider.GetService<IFileSystem>().CreateIfNotExist();

        // as world manager
        register<SafeDictionary<long, IWorld>>(new SafeDictionary<long, Map.IWorld>());
        register<SafeDictionary<Guuid, IWorldFactory>>(new SafeDictionary<Guuid, IWorldFactory>());

        provider.TryRegisterService(builder);

        if (option.DatabaseSource == null)
        {
            // throw new ArgumentException("Database Source option need to be set");
        }
        // TODO:ADD PGSQL

        return provider;

        void register<T>(object instance) where T : notnull
        {
            provider.TryRegisterService<T>((T)instance);
            builder.RegisterInstance(instance).As<T>().ExternallyOwned();
        }
    }

    /// <summary>
    /// 加载存档
    /// </summary>
    /// <param name="provider"></param>
    static void _LoadSave(Core.IServiceProvider provider)
    {
        var array = provider.GetService<SafeDictionary<Guuid, IWorldFactory>>().ToArray();

        // TODO: Load saves from file
        if (array.Length == 1)
        {
            provider.GetService<SafeDictionary<long, IWorld>>().TryAdd(0, array[0].Value.GenerateNewWorld());
        }
    }

    /// <summary>
    /// 启动
    /// </summary>
    /// <param name="provider"></param>
    static void _StartLogicThread(Utopia.Core.IServiceProvider provider)
    {
        var logicT = new Thread(() =>
        {
            var t = provider.GetService<ILogicThread>();
            var worlds = provider.GetService<SafeDictionary<long, IWorld>>().ToArray();
            foreach (var world in worlds)
            {
                t.AddUpdatable(world.Value);
            }
            // 注册关闭事件
            provider.GetService<IEventBus>().Register<LifeCycleEvent<LifeCycle>>((cycle) =>
            {
                if (cycle.Cycle == LifeCycle.Stop)
                {
                    t.Stop();
                }
            });
            t.Run();
        })
        {
            Name = "Server Logic Thread"
        };
        logicT.Start();
    }

    /// <summary>
    /// 启动网络线程
    /// </summary>
    static void _StartNetworkThread(Core.IServiceProvider provider, int port)
    {
        var netServer = provider.GetService<IInternetListener>();
        provider.TryRegisterService(netServer);
        netServer.Listen(port);
        _logger.Info("listen to {port}", port);

        var netThread = provider.GetService<InternetMain>();
        var thread = new Thread(() =>
        {
            // 注册关闭事件
            provider.GetService<IEventBus>().Register<LifeCycleEvent<LifeCycle>>((cycle) =>
            {
                if (cycle.Cycle == LifeCycle.Stop)
                {
                    netThread.Stop();
                }
            });
            netThread.Run();
        })
        {
            Name = "Server Networking Thread"
        };
        thread.Start();
    }

    static void Main(string[] args)
    {
        LaunchWithArguments(args, null);
    }
}
