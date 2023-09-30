#region copyright
// This file(may named InternetMain.cs) is a part of the project: Utopia.Server.
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

using CommunityToolkit.Diagnostics;
using System.Net.Sockets;
using Utopia.Core.Collections;
using Utopia.Core.Events;
using Utopia.Core.Exceptions;
using Utopia.G.Net;

namespace Utopia.Server.Net;

/// <summary>
/// 网络线程
/// </summary>
public sealed class InternetMain
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
        lock (this._lock)
        {
            if (this._running)
            {
                throw new InvalidOperationException("the thread has started");
            }
            this._running = true;
        }

        while (this._running)
        {
            // wait for accept
            var net = this._serviceProvider.GetService<IInternetListener>();

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
            var e = new ComplexEvent<Socket, IConnectHandler>(socket, null);
            this.ClientCreatedEvent.Fire(e);
            var client = e.Result ??
                throw new EventAssertionException(EventAssertionFailedCode.ResultIsNull);

            lock (this._lock)
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
            Task.Run(client.InputLoop);
        }

    }

    public void Stop()
    {
        lock (this._lock)
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
