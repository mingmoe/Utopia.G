//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utopia.Core.Net
{
    /// <summary>
    /// 通道工厂实现
    /// </summary>
    public class ChannelFactory : IChannelFactory
    {
        /// <summary>
        /// Handler工厂
        /// </summary>
        public List<Func<ISocket, IHandler>> HandlerFactories { get; } = new();

        public IChannel Create(ISocket socket)
        {
            var channel = new Channel();
            channel.Handlers.Add(new SocketHandle(socket));

            HandlerFactories.ForEach((f) => { channel.Handlers.Add(f.Invoke(socket)); });

            return channel;
        }
    }
}
