using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Utopia.Core;
using Utopia.Core.Exception;
using Utopia.G.Net;

namespace Utopia.Server.Net;

/// <summary>
/// 网络线程
/// </summary>
public class NetThread
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

    public NetThread(Core.IServiceProvider serviceProvider)
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
            var net = this._serviceProvider.GetService<INetServer>();

            var accept = net.Accept();
            while (this._running && !accept.IsCompleted)
            {
                Thread.Yield();
            }
            var socket = accept.Result;

            var e = new ComplexEvent<Socket, IConnectHandler>(socket, null, false);
            ClientCreatedEvent.Fire(e);
            var client = e.Result ??
                throw new EventAssertionException(EventAssertionFailedCode.ResultIsNull);

            lock (_lock)
            {
                this._clients.Add(client);
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
