// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Autofac.Core;

namespace Utopia.Core.Net;
public class StandardSocket(Socket socket) : ISocket
{
    private bool _disposed = false;

    private readonly Socket _socket = socket;

    public bool Alive { get; private set; } = socket.Connected;

    public async Task SocketMaintainer()
    {
        while (!_disposed)
        {
            // try ping
            try
            {
                Ping pingTest = new();
                PingReply reply = await pingTest.SendPingAsync(
                    (_socket.RemoteEndPoint as IPEndPoint)?.Address
                    ?? throw new IOException("Socket.RemoteEndPoint returns null"),
                    // wait 2 seconds
                    new TimeSpan(0, 0, 2));

                if (reply.Status != IPStatus.Success)
                {
                    Alive = false;
                    return;
                }
            }
            catch (Exception)
            {
                Alive = false;
                return;
            }

            // check tcp
            bool part1 = _socket.Poll(2000, SelectMode.SelectRead);
            bool part2 = (_socket.Available == 0);

            if (part1 && part2)
                Alive = false;
            else
                Alive = true;

            // ping per five seconds
            await Task.Delay(1000 * 5);
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
            _socket.Dispose();
        }

        _disposed = true;
    }

    public async Task<int> Read(Memory<byte> dst)
    {
        return await _socket.ReceiveAsync(dst);
    }

    public void Shutdown()
    {
        _socket.Shutdown(SocketShutdown.Both);
    }

    public async Task Write(ReadOnlyMemory<byte> data)
    {
        await _socket.SendAsync(data);
    }
}
