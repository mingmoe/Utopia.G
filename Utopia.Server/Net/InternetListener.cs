// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Net;
using System.Net.Sockets;
using Utopia.Core.Events;

namespace Utopia.Server.Net;
public class InternetListener : IInternetListener
{

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

    public IEventManager<ComplexEvent<Socket, Socket>> AcceptEvent { get; } =
        new EventManager<ComplexEvent<Socket, Socket>>();

    public async Task<Socket> Accept()
    {
        Socket socket;
        lock (_lock)
        {
            if (_socket == null)
            {
                throw new InvalidOperationException("the server is not working now!");
            }
            socket = _socket;
        }
        _ = await socket.AcceptAsync();

        var e = new ComplexEvent<Socket, Socket>(socket, null);
        AcceptEvent.Fire(e);
        Socket newSocket = Event.GetResult<IEventWithResult<Socket>, Socket>(e);

        return newSocket;
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

            var listenSocket = new Socket(AddressFamily.InterNetwork,
                                     SocketType.Stream,
                                     ProtocolType.Tcp);

            // bind the listening socket to the port
            var ep = new IPEndPoint(IPAddress.Loopback, port);
            listenSocket.Bind(ep);

            // start listening
            listenSocket.Listen(port);

            _socket = listenSocket;
        }
    }

    public void Shutdown()
    {

        lock (_lock)
        {
            if (_socket == null)
            {
                throw new InvalidOperationException("the server is not working now!");
            }

            _socket.Close();
            _socket.Dispose();
            _port = null;
        }
    }

    public void Dispose()
    {
        Shutdown();
        GC.SuppressFinalize(this);
    }
}
