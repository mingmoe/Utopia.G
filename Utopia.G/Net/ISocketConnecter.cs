#region copyright
// This file(may named ISocketConnecter.cs) is a part of the project: Utopia.G.
// 
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// 
// This file is part of Utopia.G.
//
// Utopia.G is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Utopia.G is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with Utopia.G. If not, see <https://www.gnu.org/licenses/>.
#endregion

using System;
using System.Net.Sockets;
using Utopia.Core.Events;

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
