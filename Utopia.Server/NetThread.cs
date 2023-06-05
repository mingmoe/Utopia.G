using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utopia.Core;

namespace Utopia.Server;

/// <summary>
/// 网络线程
/// </summary>
public class NetThread
{
    private readonly object _lock = new();
    private readonly List<IClient> _clients = new();
    private volatile bool _running = false;

    readonly Utopia.Core.IServiceProvider _serviceProvider;

    public readonly IEventManager<Event<IClient, IClient>> ClientCreatedEvent = new
        EventManager<Event<IClient, IClient>, IClient, IClient>();

    public NetThread(Utopia.Core.IServiceProvider serviceProvider)
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
            while (this._running && (!accept.IsCompleted))
            {
                Thread.Yield();
            }
            IClient client = new Client(accept.Result, this._serviceProvider);

            var e = new Event<IClient, IClient>(client, null, false);
            ClientCreatedEvent.Fire(e);
            client = e.Result ?? client;

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
            foreach (var client in this._clients)
            {
                client.Disconnect();
            }
            this._clients.Clear();
        }
    }

}
