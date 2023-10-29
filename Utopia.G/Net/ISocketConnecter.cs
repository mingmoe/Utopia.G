// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.Net.Sockets;
using Utopia.Core.Events;
using Utopia.Core.Net;

namespace Utopia.G.Net;

/// <summary>
/// This class can connect to the game server.
/// It also take responsibility for maintainning the socket link.
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
    IConnectHandler Connect(Uri url) => Connect(url.Host, url.Port);

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
