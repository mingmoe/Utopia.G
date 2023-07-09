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
/// 链接持有器.
/// </summary>
public interface IConnectHandler : IDisposable
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

    void Disconnect();
}

public class ConnectHandler : IConnectHandler
{
    private volatile bool _running = false;
    private readonly object _lock = new();
    private readonly Socket _socket;
    private readonly NetworkStream _stream;
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public ConnectHandler(Socket socket)
    {
        Guard.IsNotNull(socket);
        this._socket = socket;
        this._stream = new(socket, true);
    }

    public IDispatcher Dispatcher { get; } = new Dispatcher();

    public Packetizer Packetizer { get; } = new();

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
        _logger.Info("client net server start to run");

        try
        {
            while (this._socket.Connected && _running)
            {
                var (id, data) = await Packetizer.ReadPacket(this._stream);

                var packet = this.Packetizer.ConvertPacket(id, data);

                this.Dispatcher.DispatchPacket(id, packet);

                await Task.Yield();
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex);
        }
        finally
        {
            _logger.Info("client net server exit");
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

    public void Dispose()
    {
        this._stream.Dispose();
        this._socket.Dispose();
        GC.SuppressFinalize(this);
    }
}
