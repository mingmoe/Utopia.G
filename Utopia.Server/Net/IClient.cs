using CommunityToolkit.Diagnostics;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Utopia.Core;
using Utopia.Core.Net;
using Utopia.Core.Net.Packet;
using Utopia.Server.Map;

namespace Utopia.Server.Net;

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
    private readonly Core.IServiceProvider _provider;
    private readonly NetworkStream _stream;
    private readonly IDispatcher _dispatcher = new Dispatcher();
    private readonly Packetizer _packetizer = new();
    private Player? _player = null;
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public Client(Socket socket, Core.IServiceProvider provider)
    {
        Guard.IsNotNull(socket);
        Guard.IsNotNull(provider);
        this._socket = socket;
        this._provider = provider;
        this._stream = new(socket, true);

        this._dispatcher.RegisterHandler(
            Utopia.Core.Net.Packet.LoginPacketFormatter.PacketTypeId,
            (obj) =>
            {
                lock (this._lock)
                {
                    if (this._player != null)
                    {
                        _logger.Error("{client} try to login more than once"
                            , ((IPEndPoint?)this._socket.RemoteEndPoint)?.Address);
                        return;
                    }
                    this._player = new Player(this, ((LoginPacket)obj).PlayerId);

                    this._provider.TryGetBlock(new WorldPosition()
                    {
                        Id = 1,
                        X = 0,
                        Y = 0,
                        Z = 0,
                    }, out IBlock? block);
                    block!.TryAddEntity(this._player);
                }
            });
        this._dispatcher.RegisterHandler(
            QueryMapPacketFormatter.PacketTypeId,
            (obj) =>
            {
                var packet = (QueryMapPacket)obj;

                if (this._provider.TryGetBlock(packet.QueryPosition, out IBlock? block))
                {
                    var entities = block!.GetAllEntities();
                    packet.Accessible = block.Accessable;
                    packet.Collidable = block.Collidable;
                    packet.Entities = entities.Select((entity) => entity.Id).ToArray();

                    // write back
                    var data = this._packetizer.WritePacket(
                        QueryMapPacketFormatter.PacketTypeId,
                        packet);

                    this.WritePacket(QueryMapPacketFormatter.PacketTypeId, data);
                }
                else
                {
                    _logger.Error("try to query unknown block {position}", packet.QueryPosition);
                }
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
        lock (this._lock)
        {
            if (this._player != null)
            {
                this._provider.TryGetBlock(this._player.WorldPosition, out var b);
                b?.RemoveEntity(this._player);
                this._player = null;
            }
        }
    }
}
