// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Net.Sockets;
using CommunityToolkit.Diagnostics;
using Utopia.Core.Collections;
using Utopia.Core.Events;
using Utopia.Core.Exceptions;
using Utopia.Core.Net;

namespace Utopia.Server.Net;

public interface IInternetMain
{
    public IEventManager<IEventWithParamAndResult<Socket, IConnectHandler>> ClientCreatedEvent { get; }

    CancellationTokenSource StopTokenSource { get; }
}

/// <summary>
/// 网络线程
/// </summary>
public sealed class InternetMain : IInternetMain
{
    private readonly SafeList<IConnectHandler> _clients = new();
    private readonly Core.IServiceProvider _serviceProvider;

    public IEventManager<IEventWithParamAndResult<Socket, IConnectHandler>> ClientCreatedEvent { get; } =
        new EventManager<IEventWithParamAndResult<Socket, IConnectHandler>>();

    public CancellationTokenSource StopTokenSource { get; } = new();

    public InternetMain(Core.IServiceProvider serviceProvider)
    {
        Guard.IsNotNull(serviceProvider);
        _serviceProvider = serviceProvider;
    }

    public void Run()
    {
        while (!StopTokenSource.IsCancellationRequested)
        {
            // wait for accept
            IInternetListener net = _serviceProvider.GetService<IInternetListener>();

            Task<Socket> accept = net.Accept();
            while (!StopTokenSource.IsCancellationRequested && !accept.IsCompleted)
            {
                _ = Thread.Yield();
            }
            if (!StopTokenSource.IsCancellationRequested)
            {
                accept.Dispose();
                return;
            }
            Socket socket = accept.Result;

            // begin to create
            var e = new ComplexEvent<Socket, IConnectHandler>(socket, null);
            ClientCreatedEvent.Fire(e);
            IConnectHandler client = e.Result ??
                throw new EventAssertionException(EventAssertionFailedCode.ResultIsNull);

            _clients.EnterSync((IList<IConnectHandler> l) =>
            {
                if (StopTokenSource.IsCancellationRequested)
                {
                    _clients.Add(client);
                }
                else
                {
                    client.Dispose();
                    socket.Dispose();
                    return;
                }

            });
            Task.Run(client.InputLoop);
        }
    }
}
