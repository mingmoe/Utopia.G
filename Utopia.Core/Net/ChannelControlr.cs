//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utopia.Core.Net
{
    /// <summary>
    /// 通道控制器，负责控制通道，检测通道和套接字状态，根据套接字状态发送各种通道调用链。
    /// </summary>
    public class ChannelControlr
    {
        static readonly Logger logger = NLog.LogManager.GetCurrentClassLogger();
        readonly IChannel channel;
        readonly ISocket socket;

        public ChannelControlr(IChannel channel, ISocket handle)
        {
            ArgumentNullException.ThrowIfNull(channel, nameof(channel));
            ArgumentNullException.ThrowIfNull(handle, nameof(handle));

            this.socket = handle;
            this.channel = channel;
        }

        /// <summary>
        /// 这个指标指示套接字是否处在链接中
        /// </summary>
        public bool IsConnected { get; private set; } = true;

        /// <summary>
        /// 这个指标只是是否还需要调用Fire函数。
        /// 在套接字关闭之后需要触发Disconnect调用链，
        /// 因此不能单纯地检查套接字是否关闭来检查是否需要继续调用Fire。
        /// </summary>
        public bool IsDone { get; private set; } = false;

        private bool FiredConnected = false;

        private bool FiredDisconnected = false;

        public async Task Fire()
        {
            if (!socket.Connected)
            {
                IsConnected = false;
                socket.Close();

                if (!FiredDisconnected)
                {
                    IsDone = true;
                    FiredDisconnected = true;
                    await channel.FireDisconnect();
                }

                return;
            }

            if (!FiredConnected)
            {
                FiredConnected = true;
                await channel.FireConnect();
            }

            await channel.FireRead();
        }

        /// <summary>
        /// 停止任务循环
        /// </summary>
        public void StopFireLoopAndDisconnect()
        {
            socket.Close();
        }

        /// <summary>
        /// 设置一个任务循环
        /// </summary>
        public void FireLoop()
        {
            var t = async () =>
            {
                await Fire();
            };

            Task.Factory.StartNew(t, CancellationToken.None,
            TaskCreationOptions.None,
            TaskScheduler.Default).ContinueWith((result) =>
            {
                if(result.IsFaulted)
                {
                    logger.Error(result.Exception, "failed to fire the channel");
                }

                if (!this.IsDone) {
                    this.FireLoop();
                }
            });
        }
    }
}
