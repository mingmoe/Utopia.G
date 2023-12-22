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
internal sealed class InternetMain : IInternetMain
{
    public required IInternetListener InternetListener { get; init; }

    private readonly SafeList<IConnectHandler> _clients = new();

    public IEventManager<IEventWithParamAndResult<Socket, IConnectHandler>> ClientCreatedEvent { get; } =
        new EventManager<IEventWithParamAndResult<Socket, IConnectHandler>>();

    public CancellationTokenSource StopTokenSource { get; } = new();

    public void Run(CancellationTokenSource startTokenSource)
    {
        while (!StopTokenSource.IsCancellationRequested)
        {
            // wait for accept
            startTokenSource.CancelAfter(100/* wait for fun :-) */);

            Task<Socket> accept = InternetListener.Accept();
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
