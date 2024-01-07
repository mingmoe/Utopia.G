// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Buffers;
using System.Net;
using System.Net.Sockets;
using Autofac.Core;
using Microsoft.Extensions.Logging;
using Utopia.Core.Collections;
using Utopia.Core.Events;
using Utopia.Core.Net;

namespace Utopia.Server.Net;

/// <summary>
/// using UDP
/// </summary>
internal class InternetListener : IInternetListener
{
    public required ILogger<InternetListener> Logger { private get; init; }

    private SafeDictionary<EndPoint, UDPSocket> _sockets = new();
    private bool _disposed = false;
    private Socket? _socket = null;
    private int? _port = null;
    private readonly object _lock = new();

    public int Port
    {
        get
        {
            lock (_lock)
            {
                return _port == null ? throw new InvalidOperationException("the net server is not working now!") : _port.Value;
            }
        }
    }

    public bool Working
    {
        get
        {
            lock (_lock)
            {
                return _socket != null;
            }
        }
    }

    public IEventManager<ComplexEvent<(Socket?, EndPoint?), ISocket>> AcceptEvent { get; } =
        new EventManager<ComplexEvent<(Socket?, EndPoint?), ISocket>>();

    public InternetListener()
    {
        Task.Run(_Maintainer);
    }

    private async Task _Maintainer()
    {
        while (!_disposed)
        {
            try
            {
                var array = _sockets.ToArray();

                foreach(var item in array)
                {
                    if(item.Value.Alive)
                    {
                        continue;
                    }

                    _sockets.TryRemove(item.Key, out _);
                }

                await Task.Delay(100);
                await Task.Yield();
            }
            catch(Exception ex)
            {
                Logger.LogError(ex, "The maintainer of the Internet Listener get an error");
            }
        }
    }

    public async Task<ISocket> Accept()
    {
        EndPoint newEndpoint;
        IMemoryOwner<byte> initialPacket;
        int receivedBytes;
        UDPSocket newSocket;
        lock (_lock)
        {
            if (_socket == null)
            {
                throw new InvalidOperationException("the server is not working now!");
            }
        }

        // receive
        var anyIp = new IPEndPoint(IPAddress.Any, Port);
        while (true)
        {
            await Task.Yield();
            IMemoryOwner<byte> rent = MemoryPool<byte>.Shared.Rent(128);
            try
            {
                var got = await _socket.ReceiveFromAsync(rent.Memory, anyIp);
                bool isNew = false;
                var socket = _sockets.GetOrAdd(got.RemoteEndPoint, (endPoint) =>
                {
                    isNew = true;
                    return new UDPSocket(endPoint);
                });

                socket.AddPacket(rent.Memory.Slice(0, got.ReceivedBytes));
                rent.Dispose();

                if (!isNew)
                {
                    continue;
                }

                // prepare add it
                newEndpoint = got.RemoteEndPoint;
                receivedBytes =got.ReceivedBytes;
                initialPacket = rent;
                newSocket = socket;
                break;
            }
            catch
            {
                rent?.Dispose();
                throw;
            }
        }

        // raise event
        try
        {
            var e = new ComplexEvent<(Socket?, EndPoint?), ISocket>(new(null, newEndpoint), new KcpSocket(newSocket));
            AcceptEvent.Fire(e);
            return Event.GetResult(e);
        }
        finally
        {
            initialPacket.Dispose();
        }
    }

    public void Listen(int port)
    {
        lock (_lock)
        {
            if (_socket != null)
            {
                throw new InvalidOperationException("the server has started!");
            }

            _port = port;

            _socket = new Socket(AddressFamily.InterNetwork,
                                     SocketType.Stream,
                                     ProtocolType.Udp);
        }
    }

    public void Shutdown()
    {
        Dispose();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            lock (_lock)
            {
                _socket?.Close();
                _socket?.Dispose();
                _port = null;
            }
        }

        _disposed = true;
    }
}
