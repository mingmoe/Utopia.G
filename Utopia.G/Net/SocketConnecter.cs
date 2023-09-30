#region copyright
// This file(may named SocketConnecter.cs) is a part of the project: Utopia.G.
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
using System.IO;
using System.Net;
using System.Net.Sockets;
using Utopia.Core.Events;
using Utopia.Core.Exceptions;

namespace Utopia.G.Net;

/// <summary>
/// 负责连接到服务器
/// </summary>
public class SocketConnecter : ISocketConnecter
{
    private readonly object _lock = new();
    private Socket? _socket = null;

    public
        IEventManager<IEventWithParamAndResult<Socket, IConnectHandler>> ConnectCreatedEvent
    { get; init; } = new
        EventManager<IEventWithParamAndResult<Socket, IConnectHandler>>();

    public IConnectHandler? ConnectHandler { get; private set; } = null;

    /// <summary>
    /// 链接到服务器
    /// </summary>
    /// <param name="host">服务器地址</param>
    /// <param name="port"></param>
    /// <exception cref="InvalidOperationException">该Client已经连接到服务器</exception>
    /// <exception cref="IOException">链接异常</exception>
    public IConnectHandler Connect(string host, int port)
    {
        ArgumentNullException.ThrowIfNull(host);

        lock (this._lock)
        {
            if (this._socket != null)
            {
                throw new InvalidOperationException("the client has connected");
            }

            // Get host related information.
            IPHostEntry? hostEntry = Dns.GetHostEntry(host);

            Socket? tempSocket = null;

            foreach (IPAddress address in hostEntry.AddressList)
            {
                IPEndPoint ipe = new(address, port);
                tempSocket =
                    new(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                tempSocket.Connect(ipe);

                if (tempSocket.Connected)
                {
                    break;
                }
                else
                {
                    continue;
                }
            }

            if (tempSocket == null)
            {
                throw new IOException("failed to connect the server:" + host);
            }

            this._socket = tempSocket;

            var @event = new ComplexEvent<Socket, IConnectHandler>(tempSocket, null, false);

            this.ConnectCreatedEvent.Fire(@event);

            this.ConnectHandler = @event.Result ??
                throw new EventAssertionException(EventAssertionFailedCode.ResultIsNull);

            return this.ConnectHandler;
        }
    }

    public void Close()
    {
        lock (this._lock)
        {
            this._socket?.Close();
            this._socket = null;
        }
    }
}
