// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Net.Sockets;
using Autofac;
using CommunityToolkit.Diagnostics;
using Utopia.Core.Collections;
using Utopia.Core.Events;
using Utopia.Core.Exceptions;
using Utopia.Core.Net;

namespace Utopia.Server.Net;

public interface IInternetMain
{
    /// <summary>
    /// The <see cref="ISocket"/> and <see cref="IConnectHandler"/> was registered to the builder.
    /// </summary>
    public IEventManager<IEventWithParam<ContainerBuilder>> ClientContainerCreateEvent { get; }

    public IEventManager<IEventWithParamAndResult<ISocket, IConnectHandler>> ClientCreatedEvent { get; }

    CancellationTokenSource StopTokenSource { get; }
}

/// <summary>
/// 网络线程
/// </summary>
internal sealed class InternetMain : IInternetMain
{
    public required IInternetListener InternetListener { get; init; }

    public required ILifetimeScope Container { get; init; }

    /// <summary>
    /// TODO: check client alive frequaently and shutdown all the clients when shutdown
    /// </summary>
    private readonly SafeList<IConnectHandler> _clients = new();

    public IEventManager<IEventWithParamAndResult<ISocket, IConnectHandler>> ClientCreatedEvent { get; } =
        new EventManager<IEventWithParamAndResult<ISocket, IConnectHandler>>();

    public IEventManager<IEventWithParam<ContainerBuilder>> ClientContainerCreateEvent { get; } =
        new EventManager<IEventWithParam<ContainerBuilder>>();

    public CancellationTokenSource StopTokenSource { get; } = new();

    public void Run(CancellationTokenSource startTokenSource)
    {
        while (!StopTokenSource.IsCancellationRequested)
        {
            // wait for accept
            startTokenSource.CancelAfter(100/* wait for fun :-) */);

            Task<ISocket> accept = InternetListener.Accept();
            while (!StopTokenSource.IsCancellationRequested && !accept.IsCompleted)
            {
                Thread.Yield();
            }
            if (StopTokenSource.IsCancellationRequested)
            {
                accept.Dispose();
                return;
            }
            var socket = accept.Result;

            // begin to create
            // construct container
            var container = Container.BeginLifetimeScope((builder) =>
            {
                builder
                .RegisterInstance(socket)
                .SingleInstance();

                builder
                .RegisterType<ConnectHandler>()
                .SingleInstance()
                .As<IConnectHandler>();

                var @event = new EventWithParam<ContainerBuilder>(builder);

                ClientContainerCreateEvent.Fire(@event);
            });
            try
            {
                var e = new ComplexEvent<ISocket, IConnectHandler>(socket, container.Resolve<IConnectHandler>());
                ClientCreatedEvent.Fire(e);
                IConnectHandler client = Event.GetResult(e);

                _clients.EnterSync((IList<IConnectHandler> l) =>
                {
                    if (!StopTokenSource.IsCancellationRequested)
                    {
                        _clients.Add(client);
                        Task.Run(client.InputLoop);
                    }
                    else
                    {
                        container.Dispose();
                        return;
                    }
                });
            }
            catch
            {
                container.Dispose();
                throw;
            }
        }
    }
}
