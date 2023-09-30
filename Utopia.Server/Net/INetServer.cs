#region copyright
// This file(may named INetServer.cs) is a part of the project: Utopia.Server.
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

using System.Net;
using System.Net.Sockets;
using Utopia.Core.Events;

namespace Utopia.Server.Net;

/// <summary>
/// 网络服务器接口，线程安全。
/// </summary>
public interface INetServer : IDisposable
{
    /// <summary>
    /// 目前正在监听的端口号。如果服务器尚未启动则引发异常。
    /// </summary>
    public int Port { get; }

    /// <summary>
    /// 检查网络服务器是否处在工作状态
    /// </summary>
    public bool Working { get; }

    public IEventManager<ComplexEvent<Socket, Socket>> AcceptEvent { get; }

    /// <summary>
    /// 链接并监听端口，一个INetServer只能监听一个端口。
    /// </summary>
    /// <param name="port">端口号</param>
    void Listen(int port);

    /// <summary>
    /// 接受新链接，必须监听一个端口号后才能调用。
    /// </summary>
    /// <returns></returns>
    Task<Socket> Accept();

    /// <summary>
    /// 停机，释放资源。
    /// </summary>
    void Shutdown();
}

public class NetServer : INetServer
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

        var e = new ComplexEvent<Socket, Socket>(socket, null, false);
        this.AcceptEvent.Fire(e);
        var newSocket = e.Result ?? e.Parameter!;

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
