// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Utopia.Core.Events;
using Utopia.Core.Exceptions;
using Utopia.Core.Net;

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

        lock (_lock)
        {
            if (_socket != null)
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

            _socket = tempSocket;

            var @event = new ComplexEvent<Socket, IConnectHandler>(tempSocket, null);

            ConnectCreatedEvent.Fire(@event);

            ConnectHandler = @event.Result ??
                throw new EventAssertionException(EventAssertionFailedCode.ResultIsNull);

            return ConnectHandler;
        }
    }

    public void Close()
    {
        lock (_lock)
        {
            _socket?.Close();
            _socket = null;
        }
    }
}
