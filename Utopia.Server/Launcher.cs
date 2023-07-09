//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//

using Autofac;
using Castle.Components.DictionaryAdapter.Xml;
using Castle.Core.Logging;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Utopia.Core;
using Utopia.Core.Translate;
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

    /// <summary>
    /// 启动参数
    /// </summary>
    public class LauncherOption
    {
        /// <summary>
        /// 服务器端口
        /// </summary>
        public int Port { get; set; } = Convert.ToInt32("114514", 8);

        /// <summary>
        /// 是否跳过初始化log系统
        /// </summary>
        public bool SkipInitLog { get; set; } = false;

        /// <summary>
        /// 是否关闭RegexLog
        /// </summary>
        public bool DisableRegexLog { get; set; } = false;

        /// <summary>
        /// 文件系统
        /// </summary>
        public IFileSystem? FileSystem { get; set; } = null;

        /// <summary>
        /// 数据库链接，如果为null，那么则使用自带的PostgreSql。
        /// 推荐使用自带的PostgreSql
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
    public static void LaunchWithArguments(string[] args)
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
            else if (arg == "--disbale-regex-log")
            {
                option.DisableRegexLog = true;
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

        Launch(option);
    }

    /// <summary>
    /// 使用参数启动服务器
    /// </summary>
    /// <param name="option">参数</param>
    public static void Launch(LauncherOption option)
    {
        ArgumentNullException.ThrowIfNull(option);

        Thread.CurrentThread.Name = "Server Initialization Thread";

        var provider = _InitSystem(option);

        var bus = provider.GetService<IEventBus>();

        try
        {
            stage(LifeCycle.InitializedSystem, () =>
            {
                _logger.Info("start to initlize the server");
            });

            // 加载插件
            stage(LifeCycle.LoadPlugin, () => { _LoadPlugin(provider); });

            // 创建世界
            stage(LifeCycle.LoadSaveings, () => { _LoadSave(provider); });

            // 设置逻辑线程
            stage(LifeCycle.StartLogicThread, () => { _StartLogicThread(provider); });

            // 设置网络线程
            stage(LifeCycle.StartNetThread, () => { _StartNetworkThread(provider, option.Port); });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "the server initlize failed");
            stage(LifeCycle.Crash, () => { });
            stage(LifeCycle.Stop, () => { });
        }

        void stage(LifeCycle cycle, Action action)
        {
            bus!.Fire(new LifeCycleEvent(LifeCycleOrder.Before, cycle));
            action.Invoke();
            bus!.Fire(new LifeCycleEvent(LifeCycleOrder.After, cycle));
        }
    }
    private static ServiceProvider _InitSystem(LauncherOption option)
    {
        Guard.IsNotNull(option);
        // 初始化日志系统
        if (!option.SkipInitLog)
        {
            Core.Logging.LogManager.Init(!option.DisableRegexLog);
        }

        // 初始化依赖注入和服务提供者
        ServiceProvider provider = new();
        ContainerBuilder builder = new();

        register<IFileSystem>(option.FileSystem ?? new FileSystem());
        register<Castle.Core.Logging.ILoggerFactory>(new Castle.Services.Logging.NLogIntegration.NLogFactory(true));
        register<Microsoft.Extensions.Logging.ILoggerFactory>(new NLog.Extensions.Logging.NLogLoggerFactory());
        register<Core.IServiceProvider>(provider);
        register<IPluginLoader<IPlugin>>(new PluginLoader<IPlugin>());
        register<ITranslateManager>(new Core.Translate.TranslateManager());
        register<TranslateIdentifence>(option.GlobalLanguage);
        register<ILogicThread>(new SimplyLogicThread());
        register<IEventBus>(new EventBus());

        // as world manager
        register<SafeDictionary<long, Map.IWorld>>(new SafeDictionary<long, Map.IWorld>());

        var container = builder.Build();
        provider.TryRegisterService<IContainer>(container);

        // init filesystem
        provider.GetService<IFileSystem>().CreateIfNotExist();

        if (option.DatabaseSource == null)
        {
            throw new ArgumentException("Database Source option need to be set");
        }

        return provider;

        void register<T>(object instance) where T : notnull
        {
            provider.TryRegisterService<T>((T)instance);
            builder.RegisterInstance(instance).As<T>().ExternallyOwned();
        }
    }

    static void _LoadPlugin(Utopia.Core.IServiceProvider provider)
    {
        var loader = provider.GetService<IPluginLoader<IPlugin>>();
        var fs = provider.GetService<IFileSystem>();
        foreach (var f in Directory.GetFiles(fs.Plugins, "*.dll", System.IO.SearchOption.AllDirectories))
        {
            var file = Path.GetFullPath(f);
            _logger.Info("loading plugin from dll:{plugin}", file);
            loader.Active(provider.GetService<IContainer>(), file);
        }
    }

    static void _LoadSave(Utopia.Core.IServiceProvider provider)
    {
        provider.GetService<SafeDictionary<long, IWorld>>().GetOrAdd(1, (_) => { return new World(1, 4, 4); });
    }

    static void _StartLogicThread(Utopia.Core.IServiceProvider provider)
    {
        var logicT = new Thread(() =>
        {
            var t = provider.GetService<ILogicThread>();
            var worlds = provider.GetService<SafeDictionary<long, Map.IWorld>>().ToArray();
            foreach (var world in worlds)
            {
                t.AddUpdatable(world.Value);
            }
            // 注册关闭事件
            provider.GetService<IEventBus>().Register<LifeCycleEvent>((cycle) =>
            {
                if (cycle.Cycle == LifeCycle.Stop)
                {
                    t.Stop();
                }
            });
            t.Run();
        })
        {
            Name = "Server Updating Thread"
        };
        logicT.Start();
    }

    static void _StartNetworkThread(Core.IServiceProvider provider, int port)
    {
        var netServer = new NetServer();
        provider.TryRegisterService<INetServer>(netServer);
        netServer.Listen(port);
        _logger.Info("listen to {port}", port);

        var netThread = new NetThread(provider);
        provider.TryRegisterService<NetThread>(netThread);
        var thread = new Thread(() =>
        {
            // 注册关闭事件
            provider.GetService<IEventBus>().Register<LifeCycleEvent>((cycle) =>
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
        LaunchWithArguments(args);
    }
}
