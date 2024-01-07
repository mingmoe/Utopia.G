// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Autofac.Core;
using Utopia.Core.Collections;

namespace Utopia.Core.Net;

public class UDPSocket : ISocket
{
    /// <summary>
    /// If this is over 5 seconds,disconnect
    /// </summary>
    private DateTime _lastGetDataTime = DateTime.UtcNow;

    private readonly object _lock = new();

    private bool _disposed = false;

    private readonly SafeList<(IMemoryOwner<byte>,int readLength,int totalLength)> _lastPacket = new();

    private Socket _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Udp);

    public bool Alive { get; private set; }

    public EndPoint? RemoteAddress { get; init; }

    public UDPSocket(EndPoint endPoint)
    {
        RemoteAddress = endPoint;
        Task.Run(SocketMaintainer);
    }

    /// <summary>
    /// Add a UDP packet
    /// </summary>
    public void AddPacket(ReadOnlyMemory<byte> memory)
    {
        if (!Alive)
        {
            throw new IOException("the socket has closed");
        }

        var got = MemoryPool<byte>.Shared.Rent(memory.Length);
        try
        {
            memory.CopyTo(got.Memory);
            
            using var @lock = _lastPacket.EnterWriteLock();
            _lastPacket.Add(new(got,0,memory.Length));
        }
        catch
        {
            got.Dispose();
        }
    }

    private async Task SocketMaintainer()
    {
        try
        {
            while (Alive && !_disposed)
            {
                // try ping
                var result = await Utilities.TryPing((_socket.RemoteEndPoint as IPEndPoint)?.Address
                    ?? throw new NotImplementedException("not implement without IpEndPoint"));

                if (result == null)
                {
                    Alive = false;
                    return;
                }

                // check time
                TimeSpan lastGetDataTime;
                lock (_lock)
                {
                    lastGetDataTime = DateTime.UtcNow - _lastGetDataTime;
                }
                if (lastGetDataTime.TotalSeconds > 5)
                {
                    Alive = false;
                }

                await Task.Delay(5 * 1000);
            }
        }
        finally
        {
            Shutdown();
        }
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
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Dispose();

            using var @lock = _lastPacket.EnterWriteLock();
            foreach (var item in _lastPacket)
            {
                item.Item1.Dispose();
            }
            _lastPacket.Clear();
        }

        _disposed = true;
        Alive = false;
    }

    public async Task<int> Read(Memory<byte> dst)
    {
        if (!Alive)
        {
            throw new IOException("the socket has closed");
        }
        // read from external
        {
            using var @lock = _lastPacket.EnterWriteLock();

            if (_lastPacket.Count != 0)
            {
                // get first
                var got = _lastPacket[0];

                var restLength = got.totalLength - got.readLength;

                var restMemory = got.Item1.Memory.Slice(got.readLength, restLength);

                var length = Math.Min(dst.Length, restMemory.Length);

                restMemory.Slice(0,length).CopyTo(dst);

                if((got.readLength + length) >= got.totalLength)
                {
                    // read all
                    _lastPacket.RemoveAt(0);
                }
                else
                {
                    // next time we read
                    _lastPacket[0] = new(got.Item1, got.readLength + length, got.totalLength);
                }

                return length;
            }
        }

        // read by ourselves
        var read = (await _socket.ReceiveFromAsync(dst, RemoteAddress!)).ReceivedBytes;

        if(read != 0)
        {
            lock (_lock)
            {
                // Flush
                _lastGetDataTime = DateTime.UtcNow;
            }
        }

        return read;
    }

    public void Shutdown()
    {
        Dispose();
    }

    public async Task Write(ReadOnlyMemory<byte> data)
    {
        if (!Alive)
        {
            throw new IOException("the socket has closed");
        }

        int sent = 0;
        while (sent < data.Length)
        {
            sent += await _socket.SendToAsync(data.Slice(sent), RemoteAddress!);
        }
    }
}
