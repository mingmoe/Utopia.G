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
using Microsoft.Extensions.Logging;
using Utopia.Core.Utilities;

namespace Utopia.Core.Net;

public class ConnectHandler : IConnectHandler
{
    public required ILogger<ConnectHandler> logger { protected get; init; }

    private volatile bool _disposed = false;

    private volatile bool _running = false;

    private readonly object _lock = new();

    private readonly ISocket _socket;

    private readonly Pipe _pipe = new();

    public bool Running
    {
        get
        {
            lock (_lock)
            {
                return _running;
            }
        }
    }

    public ConnectHandler(ISocket socket)
    {
        Guard.IsNotNull(socket);
        _socket = socket;

        if (!_socket.Alive)
        {
            throw new ArgumentException("the socket haven't connect yet");
        }
    }

    public IDispatcher Dispatcher { get; } = new Dispatcher();

    public IPacketizer Packetizer { get; } = new Packetizer();

    private async Task _ReadLoop(CancellationToken token)
    {
        PipeWriter writer = _pipe.Writer;

        // true for continue
        bool check()
        {
            return !token.IsCancellationRequested;
        }

        while (check())
        {
            Memory<byte> memory = writer.GetMemory(256);

            int bytesRead = await _socket.Read(memory);

            writer.Advance(bytesRead);

            await writer.FlushAsync();
            await Task.Yield();
        }
        await writer.CompleteAsync();
    }

    private async Task _ProcessLoop(CancellationToken token)
    {
        PipeReader reader = _pipe.Reader;
        byte[] fourBytesBuf = new byte[4];

        // true for continue
        bool check()
        {
            return !token.IsCancellationRequested;
        }

        async Task<int?> readInt()
        {
            ReadResult got;
            while (true)
            {
                try
                {
                    got = await reader.ReadAtLeastAsync(4, token);
                }
                catch (OperationCanceledException)
                {
                    // canceled
                    return null;
                }

                if (!got.IsCompleted && !got.IsCanceled)
                {
                    // read all
                    break;
                }

                if (!check())
                {
                    return null;
                }
            }

            ReadOnlySequence<byte> buf = got.Buffer.Slice(0, 4);

            buf.CopyTo(fourBytesBuf);

            int num = BitConverter.ToInt32(fourBytesBuf);
            // 从网络端序转换过来
            num = IPAddress.NetworkToHostOrder(num);

            reader.AdvanceTo(buf.End);

            return num;
        }

        while (check())
        {
            // check we have packet to read
            var hasPacket = await readInt();

            if (!hasPacket.HasValue)
            {
                if (check())
                {
                    continue;
                }
                break;
            }

            int length = hasPacket.Value;
            int strLength = (await readInt()).Value;

            // read id
            ReadResult got = await reader.ReadAtLeastAsync(strLength);

            var id = Guuid.Parse(Encoding.UTF8.GetString(got.Buffer.Slice(0, strLength)));

            reader.AdvanceTo(got.Buffer.Slice(0, strLength).End);
            // read packet
            got = await reader.ReadAtLeastAsync(length);

            object packet = Packetizer.ConvertPacket(id, got.Buffer.Slice(0, length));

            reader.AdvanceTo(got.Buffer.GetPosition(length));
            // release
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
        using CancellationTokenSource source = new();
        Task all = Task.WhenAll(_ReadLoop(source.Token), _ProcessLoop(source.Token));

        while (_socket.Alive)
        {
            if (all.IsCompleted)
            {
                break;
            }
            await Task.Yield();
        }

        source.Cancel();

        try
        {
            await all;
        }
        catch(Exception e)
        {
            logger.LogError(e, "Socket Connection Error");
        }

        // shutdown
        lock (_lock)
        {
            _running = false;
        }
    }

    private void _UnlockWrite(Memory<byte> bytes) => _socket.Write(bytes);

    public void WritePacket(Guuid packetTypeId, object obj)
    {
        var data = Packetizer.WritePacket(packetTypeId, obj);

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

            _UnlockWrite(length);
            _UnlockWrite(idLength);
            _UnlockWrite(encoderedId);
            _UnlockWrite(data);
        }
    }

    public void Disconnect()
    {
        _socket.Shutdown();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _socket.Shutdown();
            _socket.Dispose();
        }

        _disposed = true;
    }
}

