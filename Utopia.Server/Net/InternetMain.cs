using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Utopia.Core;
using Utopia.Core.Collections;
using Utopia.Core.Events;
using Utopia.Core.Exceptions;
using Utopia.G.Net;

namespace Utopia.Server.Net;

/// <summary>
/// 网络线程
/// </summary>
public class InternetMain
{
    private readonly object _lock = new();
    private readonly SafeList<IConnectHandler> _clients = new();
    private volatile bool _running = false;

    readonly Core.IServiceProvider _serviceProvider;

    /// <summary>
    /// 链接创建事件.传入一个<see cref="Socket"/>,
    /// 要求传出一个非空的<see cref="IConnectHandler"/>
    /// </summary>
    public readonly IEventManager<ComplexEvent<Socket, IConnectHandler>> ClientCreatedEvent = new
        EventManager<ComplexEvent<Socket, IConnectHandler>>();

    public InternetMain(Core.IServiceProvider serviceProvider)
    {
        Guard.IsNotNull(serviceProvider);
        this._serviceProvider = serviceProvider;
    }

    public void Run()
    {
        lock (_lock)
        {
            if (_running)
            {
                throw new InvalidOperationException("the thread has started");
            }
            this._running = true;
        }

        while (this._running)
        {
            // wait for accept
            var net = this._serviceProvider.GetService<INetServer>();

            var accept = net.Accept();
            while (this._running && !accept.IsCompleted)
            {
                Thread.Yield();
            }
            if (!this._running)
            {
                accept.Dispose();
                return;
            }
            var socket = accept.Result;

            // begin to create
            var e = new ComplexEvent<Socket, IConnectHandler>(socket, null, false);
            ClientCreatedEvent.Fire(e);
            var client = e.Result ??
                throw new EventAssertionException(EventAssertionFailedCode.ResultIsNull);

            lock (_lock)
            {
                if (this._running)
                {
                    this._clients.Add(client);
                }
                else
                {
                    client.Dispose();
                    socket.Dispose();
                    return;
                }
            }
            _ = client.InputLoop();
        }

    }

    public void Stop()
    {
        lock (_lock)
        {
            this._running = false;
            var clients = this._clients.ToArray();
            foreach (var client in clients)
            {
                client.Disconnect();
                client.Dispose();
            }
            this._clients.Clear();
        }
    }

}
