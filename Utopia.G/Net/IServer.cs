using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utopia.Core.Net;
using Utopia.Core;
using CommunityToolkit.Diagnostics;
using NLog;
using System.Net.Sockets;
using System.Net;
using Utopia.Core.Net.Packet;

namespace Utopia.G.Net;

/// <summary>
/// 服务器
/// </summary>
public interface IServer
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
    /// 服务端进行信息循环
    /// </summary>
    Task InputLoop();
}

public class Server : IServer
{
    private volatile bool _running = false;
    private readonly object _lock = new();
    private readonly Socket _socket;
    private readonly Core.IServiceProvider _provider;
    private readonly NetworkStream _stream;
    private readonly IDispatcher _dispatcher = new Dispatcher();
    private readonly Packetizer _packetizer = new();
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public Server(Socket socket, Core.IServiceProvider provider)
    {
        Guard.IsNotNull(socket);
        Guard.IsNotNull(provider);
        this._socket = socket;
        this._provider = provider;
        this._stream = new(socket, true);

        this._dispatcher.RegisterHandler(
            QueryMapPacketFormatter.PacketTypeId,
            (obj) =>
            {
                var packet = (QueryMapPacket)obj;

                
            });
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
            var (id, data) = await Packetizer.ReadPacket(this._stream);

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
