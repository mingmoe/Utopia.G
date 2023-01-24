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

namespace Utopia.Core.Net;

/// <summary>
/// 通道工厂实现
/// </summary>
public class ChannelFactory : IChannelFactory
{
    /// <summary>
    /// Handler工厂
    /// </summary>
    public List<Func<ISocket, IHandler>> HandlerFactories { get; } = new();

    public Func<ISocket, IChannelRoot> ChannelRootFactory = (s) => { return new SocketChannelRoot(s); };

    public IChannel Create(ISocket socket)
    {
        var channel = new Channel(ChannelRootFactory.Invoke(socket))
        {
            ChannelId = socket.SocketAddress
        };

        this.HandlerFactories.ForEach((f) => { channel.Handlers.Add(f.Invoke(socket)); });

        return channel;
    }
}
