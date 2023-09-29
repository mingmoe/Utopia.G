//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//

using System;
using System.Net.Sockets;
using Utopia.Core.Events;

namespace Utopia.G.Net;

/// <summary>
/// 客户端，负责连接到服务器
/// </summary>
public interface ISocketConnecter
{
    /// <summary>
    /// 连接到服务器
    /// </summary>
    /// <param name="hostname">服务器地址</param>
    /// <param name="port">服务器端口</param>
    IConnectHandler Connect(string hostname, int port);

    /// <summary>
    /// 链接到服务器
    /// </summary>
    /// <param name="url">服务器URL</param>
    IConnectHandler Connect(Uri url)
    {
        return this.Connect(url.Host, url.Port);
    }

    IConnectHandler? ConnectHandler { get; }

    /// <summary>
    /// 链接被创建事件.传入一个<see cref="Socket"/>,要求传出一个
    /// <see cref=" IConnectHandler"/>,事件不可取消
    /// </summary>
    IEventManager<IEventWithParamAndResult<Socket, IConnectHandler>> ConnectCreatedEvent
    { get; }

    /// <summary>
    /// 结束链接
    /// </summary>
    void Close();
}
