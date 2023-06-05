using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Utopia.Core;
using Utopia.Core.Net;

namespace Utopia.Server;

/// <summary>
/// 客户端，实现要求为线程安全。
/// </summary>
public interface IClient
{
    /// <summary>
    /// 包分发器
    /// </summary>
    IDispatcher Dispatcher { get; }

    /// <summary>
    /// 包格式化器
    /// </summary>
    Packetizer Packetizer { get; }

    void Write(byte[] bytes);

    void WritePacket(Guuid packetTypeId, byte[] data);

    /// <summary>
    /// 客户端进行信息循环
    /// </summary>
    Task InputLoop();

    void Disconnect();
}

public class Client : IClient
{
    private volatile bool _running = false;
    private readonly object _lock = new();
    private readonly Socket _socket;
    private readonly Utopia.Core.IServiceProvider _provider;
    private readonly NetworkStream _stream;
    private readonly IDispatcher _dispatcher = new Dispatcher();
    private readonly Packetizer _packetizer = new();

    public Client(Socket socket, Utopia.Core.IServiceProvider provider)
    {
        Guard.IsNotNull(socket);
        Guard.IsNotNull(provider);
        this._socket = socket;
        this._provider = provider;
        this._stream = new(socket, true);
    }

    public IDispatcher Dispatcher => this._dispatcher;

    public Packetizer Packetizer => this._packetizer;

    public async Task InputLoop()
    {
        lock (this._lock)
        {
            if (_running)
            {
                return;
            }
            _running = true;
        }

        while (this._socket.Connected)
        {
            var (id, data) = await Utopia.Core.Net.Packetizer.ReadPacket(this._stream);

            var packet = this._packetizer.ConvertPacket(id, data);

            this._dispatcher.DispatchPacket(id, packet);

            await Task.Yield();
        }
    }
    public void Write(byte[] bytes)
    {
        lock (_lock)
        {
            this._stream.Write(bytes);
        }
    }

    public void WritePacket(Guuid packetTypeId, byte[] data)
    {
        lock (_lock)
        {
            this._stream.Write(this.Packetizer.WritePacket(packetTypeId, data));
        }
    }

    public void Disconnect()
    {
        this._socket.Shutdown(SocketShutdown.Both);
        this._socket.Close();
    }
}
