using CommunityToolkit.Diagnostics;
using CommunityToolkit.HighPerformance;
using NLog;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Utopia.Core.Utilities;
using Utopia.G.Net;

namespace Utopia.Core.Net;

public class ConnectHandler : IConnectHandler
{
    private volatile bool _running = false;

    private readonly object _lock = new();

    private readonly Socket _socket;

    private readonly Pipe _pipe = new();

    public ConnectHandler(Socket socket)
    {
        Guard.IsNotNull(socket);
        this._socket = socket;

        if(!this._socket.Connected)
        {
            throw new ArgumentException("the socket haven't connect yet");
        }
    }

    public IDispatcher Dispatcher { get; } = new Dispatcher();

    public Packetizer Packetizer { get; } = new();

    private async Task _ReadLoop()
    {
        var writer = _pipe.Writer;

        while (this._running)
        {
            Memory<byte> memory = writer.GetMemory(512);
            int bytesRead = await this._socket.ReceiveAsync(memory, SocketFlags.None);

            writer.Advance(bytesRead);

            await Task.Yield();
        }
        await writer.CompleteAsync();
    }

    private async Task _ProcessLoop()
    {
        async Task<int> readInt(PipeReader reader)
        {
            var got = await reader.ReadAtLeastAsync(4);

            var buf = got.Buffer.Slice(0, 4);

            var span = new byte[4];
            buf.CopyTo(span);

            var num = BitConverter.ToInt32(span);
            // 从网络端序转换过来
            num = IPAddress.NetworkToHostOrder(num);

            reader.AdvanceTo(got.Buffer.GetPosition(4));

            return num;
        }

        var reader = _pipe.Reader;

        while (this._running)
        {
            var length = await readInt(reader);
            var strLength = await readInt(reader);

            // read id
            var got = await reader.ReadAtLeastAsync(strLength);

            var data = new byte[strLength];

            got.Buffer.CopyTo(data);

            reader.AdvanceTo(got.Buffer.GetPosition(strLength));

            var id = Guuid.ParseString(Encoding.UTF8.GetString(data));

            // read packet
            got = await reader.ReadAtLeastAsync(length);

            data = new byte[length];

            got.Buffer.CopyTo(data);

            reader.AdvanceTo(got.Buffer.GetPosition(length));

            // release
            var packet = this.Packetizer.ConvertPacket(id, data);

            this.Dispatcher.DispatchPacket(id,packet);
        }
    }

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

        // wait for shutdown sign
        while(this._socket.Connected && this._running)
        {
            await Task.Yield();
        }

        // shutdown
        lock (this._lock)
        {
            _running = false;
        }

        await Task.WhenAll(this._ReadLoop(), this._ProcessLoop());
    }

    private void _Write(byte[] bytes)
    {
        this._socket.Send(bytes);
    }

    public void Write(byte[] bytes)
    {
        lock (_lock)
        {
            this._Write(bytes);
        }
    }

    public void WritePacket(Guuid packetTypeId, byte[] data)
    {
        lock (_lock)
        {
            var encoderedId = Encoding.UTF8.GetBytes(packetTypeId.ToString());

            // 转换到网络端序
            var length = BitConverter.GetBytes(
                    IPAddress.HostToNetworkOrder(data.Length)
                );

            var idLength = BitConverter.GetBytes(
                    IPAddress.HostToNetworkOrder(encoderedId.Length)
                );

            this._Write(length);
            this._Write(idLength);
            this._Write(encoderedId);
            this._Write(data);
        }
    }

    public void Disconnect()
    {
        this._socket.Shutdown(SocketShutdown.Both);
        this._socket.Close();
        this._pipe.Reset();
    }

    public void Dispose()
    {
        this.Disconnect();
        this._socket.Dispose();
        GC.SuppressFinalize(this);
    }
}

