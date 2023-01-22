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
using Castle.Services.Logging.NLogIntegration;
using NLog;
using Utopia.Core;
using Utopia.Core.Net;

namespace Utopia.Server
{
    /// <summary>
    /// 服务器启动器
    /// </summary>
    public static class Launcher
    {

        /// <summary>
        /// 启动参数
        /// </summary>
        public class LauncherOption
        {

            /// <summary>
            /// 服务器端口
            /// </summary>
            public int Port { get; set; } = 23344;

            /// <summary>
            /// 是否跳过初始化log系统
            /// </summary>
            public bool SkipInitLog { get; set; } = false;

            /// <summary>
            /// 是否关闭RegexLog
            /// </summary>
            public bool DisableRegexLog { get; set; } = false;

            /// <summary>
            /// 如果为非null，则直接连接到此客户端。
            /// </summary>
            public ISocket? ClientSocket { get; set; } = null;
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
                Core.LogManager.Init(!option.DisableRegexLog);
            }

            // 初始化依赖注入和服务提供者
            ServiceProvider provider = new();
            ContainerBuilder builder = new();

            void Register<T>(object instance) where T : notnull
            {
                provider.TryRegisterService<T>((T)instance);
                builder.RegisterInstance(instance).As<T>().ExternallyOwned();
            }

            Register<IChannelFactory>(new ChannelFactory());
            Register<IFileSystem>(new FileSystem());
            Register<ILoggerFactory>(new NLogFactory(true));
            Register<Core.IServiceProvider>(provider);
            Register<IPluginLoader>(new PluginLoader());

            builder.Register((c, p) =>
            {
                return provider.GetService<ILoggerFactory>().Create(c.GetComponentType().FullName);
            }).ExternallyOwned();

            var container = builder.Build();

            // 加载插件
            var loader = provider.GetService<IPluginLoader>();
            var fs = provider.GetService<IFileSystem>();
            foreach (var f in Directory.GetFiles(fs.Plugins, "*.dll", SearchOption.AllDirectories))
            {
                loader.Load(f);
            }
            loader.Active(container);

            // 加载存档


            // 设置逻辑线程


            // 设置网络线程


            // 

        }

        static void Main(string[] args)
        {
            LaunchWithArguments(args);
        }
    }
}
