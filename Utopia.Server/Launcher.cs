//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//

using Utopia.Core;

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

            public int Port { get; set; } = 23344;

            public bool SkipInitLog { get; set; } = false;

            public bool DisableRegexLog { get; set; } = false;

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

            Thread.CurrentThread.Name = "ServerMain";

            if (!option.SkipInitLog)
            {
                LogManager.Init(!option.DisableRegexLog);
            }


        }

        static void Main(string[] args)
        {
            LaunchWithArguments(args);
        }
    }
}
