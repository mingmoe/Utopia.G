// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Autofac;
using Utopia.Core.Events;
using Utopia.Core.Exceptions;
using Utopia.Core.Net;

namespace Utopia.G.Net;

/// <summary>
/// 负责连接到服务器,use UDP && KCP
/// </summary>
public class SocketConnecter : ISocketConnecter
{
    public required ILifetimeScope Container { private get; init; }

    private readonly object _lock = new();
    private ISocket? _socket = null;

    public
        IEventManager<IEventWithParamAndResult<ISocket, IConnectHandler>> ConnectCreatedEvent
    { get; init; } = new
        EventManager<IEventWithParamAndResult<ISocket, IConnectHandler>>();

    public IConnectHandler? ConnectHandler { get; private set; } = null;

    public IEventManager<IEventWithParam<ContainerBuilder>> ConnectionContainerBuildingEvent { get; init; }
    = new EventManager<IEventWithParam<ContainerBuilder>>();

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

            _socket = new KcpSocket(new UDPSocket(new IPEndPoint(IPAddress.Parse(host), port)));

            // build container
            try
            {
                var container = Container.BeginLifetimeScope((builder) =>
                {
                    builder.RegisterInstance(_socket).As<ISocket>();

                    builder
                    .RegisterType<ConnectHandler>()
                    .SingleInstance()
                    .As<IConnectHandler>();

                    var @event = new EventWithParam<ContainerBuilder>(builder);
                    ConnectionContainerBuildingEvent.Fire(@event);
                });
                // connect
                try
                {
                    var @event = new ComplexEvent<ISocket, IConnectHandler>(_socket, container.Resolve<IConnectHandler>());

                    ConnectCreatedEvent.Fire(@event);

                    ConnectHandler = Event.GetResult(@event);

                    return ConnectHandler;
                }
                catch
                {
                    container.Dispose();
                    throw;
                }
            }
            catch
            {
                _socket.Dispose();
                throw;
            }
         }
    }

    public void Close()
    {
        lock (_lock)
        {
            _socket?.Shutdown();
            _socket = null;
        }
    }
}
