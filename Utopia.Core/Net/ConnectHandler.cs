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
using Utopia.Core.Events;
using Utopia.Core.Utilities;

namespace Utopia.Core.Net;

public class ConnectHandler : IConnectHandler
{
    public required ILogger<ConnectHandler> Logger { protected get; init; }

    /// <summary>
    /// once true,never false
    /// </summary>
    private volatile bool _eventFired = false;

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

    public IEventManager<IEventWithParam<Exception?>> SocketDisconnectEvent { get; } = 
        new EventManager<IEventWithParam<Exception?>>();

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
            new Task(async () =>
            {
                try
                {
                    var handled = await Dispatcher.DispatchPacket(id, packet);

                    if (!handled)
                    {
                        Logger.LogWarning("Packet with id {} has no handler", id);
                    }
                }
                catch(Exception ex)
                {
                    Logger.LogError(ex,"Error when handle packet {}", id);
                }
            }).Start();
        }
    }

    public async Task InputLoop()
    {
        lock (_lock)
        {
            // if it is running now or the disconnect event was fired,not start again
            if (_running || _eventFired)
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
            Logger.LogError(e, "Socket Connection Error");

            // fire event
            lock (_lock)
            {
                if (!_eventFired)
                {
                    _eventFired = true;
                    EventWithParam<Exception?> @event = new(e);
                    SocketDisconnectEvent.Fire(@event);
                }
            }
        }

        // shutdown
        lock (_lock)
        {
            _running = false;

            // fire event
            if (!_eventFired)
            {
                _eventFired = true;
                EventWithParam<Exception?> @event = new(null);
                SocketDisconnectEvent.Fire(@event);
            }

            Disconnect();
        }
    }

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

            _socket.Write(length);
            _socket.Write(idLength);
            _socket.Write(encoderedId);
            _socket.Write(data);
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

