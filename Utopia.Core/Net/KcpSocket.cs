// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Hashing;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net.Sockets.Kcp;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Utopia.Core.Net;
public sealed class KcpSocket : ISocket, IKcpCallback
{
    private bool _disposed = false;

    private Task _task = Task.CompletedTask;

    private CancellationTokenSource _updateLoopCancellationTokenSource = new();

    private SpinLock _lock = new();

    private readonly DateTimeOffset _originDate = new();

    private readonly Stopwatch _stopwatch = new();

    private DateTimeOffset Current {
        get
        {
            return _originDate.AddMilliseconds(_stopwatch.ElapsedMilliseconds);
        }
    }

    public bool Alive { get; private set; } = true;

    public EndPoint? RemoteAddress => _socket.RemoteAddress;

    private readonly ISocket _socket;

    private readonly PoolSegManager.Kcp _kcp;

    public static readonly Lazy<uint> KcpConv = new(() =>
    {
        var version = VersionUtility.UtopiaCoreVersion;

        var got = XxHash32.Hash(Encoding.UTF8.GetBytes(version.ToString()));

        return BitConverter.ToUInt32(got);
    },true);

    public KcpSocket(ISocket socket)
    {
        _socket = socket;

        _kcp = new PoolSegManager.Kcp(KcpConv.Value,this);

        SwitchFastMode();

        _stopwatch.Start();

        StartInputLoop();
    }

    public void SwitchFastMode()
    {
        _kcp.NoDelay(1, 10, 2, 1);
    }

    public void SwitchNormalMode()
    {
        _kcp.NoDelay(0, 40, 0, 0);
    }

    public Task<int> Read(Memory<byte> dst)
    {
        // receive is not thread-safe
        bool locked = false;
        try
        {
            _lock.Enter(ref locked);

            var get = _kcp.Recv(dst.Span);

            if(get < 0)
            {
                return Task.FromResult(0);
            }

            return Task.FromResult(get);
        }
        finally
        {
            if(locked)
                _lock.Exit();
        }
    }

    public async Task Write(ReadOnlyMemory<byte> data)
    {
        // send is thread-safe
        int index = 0;
        while (true)
        {
            var err = _kcp.Send(data.Slice(index).Span);

            if(err == -1)
            {
                throw new IOException("Kcp.Send returns -1, means error");
            }

            if(err > 0)
            {
                index += err;
            }

            if(index >= data.Length)
            {
                return;
            }

            await Task.Yield();
        }
    }

    /// <summary>
    /// This should run only in one thread.
    /// This method will read from socket and update kcp frequently.
    /// </summary>
    private async Task UpdateLoop(CancellationToken token)
    {
        // read data from socket
        async Task UpdateData()
        {
            var rent = ArrayPool<byte>.Shared.Rent(1024);

            try
            {
                // receive is not thread-safe
                // but we ensure that only we are call that
                var length = await _socket.Read(rent);

                // while input is thread-safe
                _kcp.Input(new Span<byte>(rent, 0, length));
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(rent);
            }
        }

        while (!token.IsCancellationRequested)
        {
            if (!Alive)
            {
                return;
            }

            var current = Current;

            DateTimeOffset next;

            // update is not thread-safe
            bool lockTaken = false;
            try
            {
                _lock.Enter(ref lockTaken);

                _kcp.Update(current);
                next = _kcp.Check(current);
            }
            finally
            {
                if(lockTaken)
                    _lock.Exit();
            }

            // wait
            while(next > Current)
            {
                if(token.IsCancellationRequested || !Alive)
                {
                    return;
                }

                await Task.Yield();
                await UpdateData();
            }

            await UpdateData();
        }
    }

    public CancellationTokenSource StartInputLoop()
    {
        // close before
        _updateLoopCancellationTokenSource.Cancel();
        _task.Wait();

        // start new
        _updateLoopCancellationTokenSource = new();
        _task = Task.Run(() => UpdateLoop(_updateLoopCancellationTokenSource.Token));
        return _updateLoopCancellationTokenSource;
    }

    public async void Output(IMemoryOwner<byte> buffer, int avalidLength)
    {
        // write nothing
        if (!_socket.Alive)
        {
            return;
        }

        using var b = buffer;
        // socket.send should be thread-safe
        await _socket.Write(b.Memory.Slice(0,avalidLength));
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _updateLoopCancellationTokenSource.Cancel();
            _socket.Shutdown();
            _socket.Dispose();
            _updateLoopCancellationTokenSource.Dispose();
        }

        Alive = false;
        _disposed = true;
    }

    public void Shutdown()
    {
        Dispose();
    }
}
