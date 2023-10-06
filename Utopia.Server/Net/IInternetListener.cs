#region copyright
// This file(may named INetServer.cs) is a part of the project: Utopia.Server.
// 
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// 
// This file is part of Utopia.Server.
//
// Utopia.Server is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Utopia.Server is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with Utopia.Server. If not, see <https://www.gnu.org/licenses/>.
#endregion

using System.Net.Sockets;
using Utopia.Core.Events;

namespace Utopia.Server.Net;

/// <summary>
/// 网络服务器接口，线程安全。
/// </summary>
public interface IInternetListener : IDisposable
{
    /// <summary>
    /// 目前正在监听的端口号。如果服务器尚未启动则引发异常。
    /// </summary>
    public int Port { get; }

    /// <summary>
    /// 检查网络服务器是否处在工作状态
    /// </summary>
    public bool Working { get; }

    public IEventManager<ComplexEvent<Socket, Socket>> AcceptEvent { get; }

    /// <summary>
    /// 链接并监听端口，一个INetServer只能监听一个端口。
    /// </summary>
    /// <param name="port">端口号</param>
    void Listen(int port);

    /// <summary>
    /// 接受新链接，必须监听一个端口号后才能调用。
    /// </summary>
    /// <returns></returns>
    Task<Socket> Accept();

    /// <summary>
    /// 停机，释放资源。
    /// </summary>
    void Shutdown();
}
