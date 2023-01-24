//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using System.Net;
using System.Net.Sockets;

namespace Utopia.Core.Net
{
    public class Socket : ISocket
    {
        readonly System.Net.Sockets.Socket _socket;

        public Socket(System.Net.Sockets.Socket socket)
        {
            ArgumentNullException.ThrowIfNull(socket, nameof(socket));
            this._socket = socket;

            var ip = socket.RemoteEndPoint as IPEndPoint;
            this.SocketAddress = string.Format("{0} Addr:{1} Port:{2}", 
                ip?.AddressFamily.ToString() ?? "unknown",
                ip?.Address?.ToString() ?? "unknown",
                ip?.Port.ToString() ?? "unknown");
        }

        public string SocketAddress { get; init; }

        public bool Connected => _socket.Connected;

        public void Close()
        {
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
            _socket.Dispose();
        }

        public void Flush()
        {

        }

        public async Task<int> Read(Memory<byte> output)
        {
            ArgumentNullException.ThrowIfNull(output, nameof(output));

            return await _socket.ReceiveAsync(output, SocketFlags.None);
        }

        public async Task Write(Memory<byte> data, int start, int length)
        {
            ArgumentNullException.ThrowIfNull(data, nameof(data));

            var s = data.Slice(start, length);

            while (true)
            {
                var read = await _socket.SendAsync(s, SocketFlags.None);

                if (read != length)
                {
                    s = data.Slice(start + read, length - read);
                }
                else
                {
                    break;
                }
            }
        }
    }
}
