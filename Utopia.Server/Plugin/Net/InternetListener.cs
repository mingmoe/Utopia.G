using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Utopia.Core.Events;
using Utopia.Server.Net;

namespace Utopia.Server.Plugin.Net;
public class InternetListener : IInternetListener 
{

    private Socket? _socket = null;
    private int? _port = null;
    private readonly object _lock = new();

    public int Port
    {
        get
        {
            lock (this._lock)
            {
                if (this._port == null)
                {
                    throw new InvalidOperationException("the net server is not working now!");
                }
                return this._port.Value;
            }
        }
    }

    public bool Working
    {
        get
        {
            lock (this._lock)
            {
                return this._socket != null;
            }
        }
    }

    public IEventManager<ComplexEvent<Socket, Socket>> AcceptEvent { get; } =
        new EventManager<ComplexEvent<Socket, Socket>>();

    public async Task<Socket> Accept()
    {
        Socket socket;
        lock (this._lock)
        {
            if (this._socket == null)
            {
                throw new InvalidOperationException("the server is not working now!");
            }
            socket = this._socket;
        }
        await socket.AcceptAsync();

        var e = new ComplexEvent<Socket, Socket>(socket, null);
        this.AcceptEvent.Fire(e);
        var newSocket = Event.GetResult<IEventWithResult<Socket>,Socket>(e);

        return newSocket;
    }

    public void Listen(int port)
    {
        lock (this._lock)
        {
            if (this._socket != null)
            {
                throw new InvalidOperationException("the server has started!");
            }

            this._port = port;

            var listenSocket = new Socket(AddressFamily.InterNetwork,
                                     SocketType.Stream,
                                     ProtocolType.Tcp);

            // bind the listening socket to the port
            var ep = new IPEndPoint(IPAddress.Loopback, port);
            listenSocket.Bind(ep);

            // start listening
            listenSocket.Listen(port);

            this._socket = listenSocket;
        }
    }

    public void Shutdown()
    {

        lock (this._lock)
        {
            if (this._socket == null)
            {
                throw new InvalidOperationException("the server is not working now!");
            }

            this._socket.Close();
            this._socket.Dispose();
            this._port = null;
        }
    }

    public void Dispose()
    {
        this.Shutdown();
        GC.SuppressFinalize(this);
    }
}
