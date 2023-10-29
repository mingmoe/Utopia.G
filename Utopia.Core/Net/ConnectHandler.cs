// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Net.Sockets;
using System.Text;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.HighPerformance;
using Utopia.Core.Utilities;

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
        _socket = socket;

        if (!_socket.Connected)
        {
            throw new ArgumentException("the socket haven't connect yet");
        }
    }

    public IDispatcher Dispatcher { get; } = new Dispatcher();

    public IPacketizer Packetizer { get; } = new Packetizer();

    private async Task _ReadLoop()
    {
        PipeWriter writer = _pipe.Writer;

        while (_running)
        {
            Memory<byte> memory = writer.GetMemory(512);
            int bytesRead = await _socket.ReceiveAsync(memory, SocketFlags.None);

            writer.Advance(bytesRead);

            await Task.Yield();
        }
        await writer.CompleteAsync();
    }

    private async Task _ProcessLoop()
    {
        static async Task<int> readInt(PipeReader reader)
        {
            ReadResult got = await reader.ReadAtLeastAsync(4);

            ReadOnlySequence<byte> buf = got.Buffer.Slice(0, 4);

            byte[] span = new byte[4];
            buf.CopyTo(span);

            int num = BitConverter.ToInt32(span);
            // 从网络端序转换过来
            num = IPAddress.NetworkToHostOrder(num);

            reader.AdvanceTo(got.Buffer.GetPosition(4));

            return num;
        }

        PipeReader reader = _pipe.Reader;

        while (_running)
        {
            int length = await readInt(reader);
            int strLength = await readInt(reader);

            // read id
            ReadResult got = await reader.ReadAtLeastAsync(strLength);

            byte[] data = new byte[strLength];

            got.Buffer.CopyTo(data);

            reader.AdvanceTo(got.Buffer.GetPosition(strLength));

            var id = Guuid.Parse(Encoding.UTF8.GetString(data));

            // read packet
            got = await reader.ReadAtLeastAsync(length);

            data = new byte[length];

            got.Buffer.CopyTo(data);

            reader.AdvanceTo(got.Buffer.GetPosition(length));

            // release
            object packet = Packetizer.ConvertPacket(id, data);

            _ = Dispatcher.DispatchPacket(id, packet);
        }
    }

    public async Task InputLoop()
    {
        lock (_lock)
        {
            if (_running)
            {
                return;
            }
            _running = true;
        }

        // wait for shutdown sign
        while (_socket.Connected && _running)
        {
            await Task.Yield();
        }

        // shutdown
        lock (_lock)
        {
            _running = false;
        }

        await Task.WhenAll(_ReadLoop(), _ProcessLoop());
    }

    private void _Write(byte[] bytes) => _socket.Send(bytes);

    public void Write(byte[] bytes)
    {
        lock (_lock)
        {
            _Write(bytes);
        }
    }

    public void WritePacket(Guuid packetTypeId, byte[] data)
    {
        lock (_lock)
        {
            byte[] encoderedId = Encoding.UTF8.GetBytes(packetTypeId.ToString());

            // 转换到网络端序
            byte[] length = BitConverter.GetBytes(
                    IPAddress.HostToNetworkOrder(data.Length)
                );

            byte[] idLength = BitConverter.GetBytes(
                    IPAddress.HostToNetworkOrder(encoderedId.Length)
                );

            _Write(length);
            _Write(idLength);
            _Write(encoderedId);
            _Write(data);
        }
    }

    public void Disconnect()
    {
        _socket.Shutdown(SocketShutdown.Both);
        _socket.Close();
        _pipe.Reset();
    }

    public void Dispose()
    {
        Disconnect();
        _socket.Dispose();
        GC.SuppressFinalize(this);
    }
}

