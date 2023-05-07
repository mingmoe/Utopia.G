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
using System.Globalization;
using System.Linq;
using Utopia.Core;
using Utopia.Core.Translate;

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

        long i = 0;
        while (i != args.LongLength)
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
        ArgumentNullException.ThrowIfNull(option, nameof(option));

        Thread.CurrentThread.Name = "Server Main";

        // 初始化日志系统
        if (!option.SkipInitLog)
        {
            Core.Logging.LogManager.Init(!option.DisableRegexLog);
        }

        // 初始化依赖注入和服务提供者
        ServiceProvider provider = new();
        ContainerBuilder builder = new();

        void register<T>(object instance) where T : notnull
        {
            provider.TryRegisterService<T>((T)instance);
            builder.RegisterInstance(instance).As<T>().ExternallyOwned();
        }

        register<IFileSystem>(option.FileSystem ?? new FileSystem());
        register<ILoggerFactory>(new Castle.Services.Logging.NLogIntegration.NLogFactory(true));
        register<Core.IServiceProvider>(provider);
        register<IPluginLoader<IPlugin>>(new PluginLoader<IPlugin>());
        register<ITranslateManager>(new Core.Translate.TranslateManager());
        register<TranslateIdentifence>(option.GlobalLanguage);
        provider.GetService<INetServer>().Listen(option.Port);

        var container = builder.Build();
        provider.TryRegisterService<IContainer>(container);

        // 设置目录
        provider.GetService<IFileSystem>().CreateIfNotExist();

        // 加载插件
        var loader = provider.GetService<IPluginLoader<IPlugin>>();
        var fs = provider.GetService<IFileSystem>();
        foreach (var f in Directory.GetFiles(fs.Plugins, "*.dll", SearchOption.AllDirectories))
        {
            loader.Active(container, f);
        }

        // 加载存档

        var w = new World(1, 4, 4);
        Dictionary<IBlock, Position> b = new();

        for (var x = -(IArea.XSize * 4); x != ((IArea.XSize * 4) - 1); x++)
        {
            for (var y = -(IArea.YSize * 4); y != ((IArea.YSize * 4) - 1); y++)
            {
                if (w.TryGetBlock(new Position() { X = x, Y = y, Z = 0 }, out IBlock? block))
                {
                    if (b.TryGetValue(block!, out Position p))
                    {
                        _logger.Error("added {0} {1} before {2} {3}", x, y, p.X, p.Y);
                    }
                    else
                    {
                        b.Add(block!, new Position() { X = x, Y = y, Z = 0 });
                    }
                }
                else
                {
                    _logger.Error("not found pos {0} {1}", x, y);
                }
            }
        }

        // 设置逻辑线程

        // TODO:设置网络线程
        // set debug handler

        // 等待

    }

    static void Main(string[] args)
    {
        LaunchWithArguments(args);
    }
}
