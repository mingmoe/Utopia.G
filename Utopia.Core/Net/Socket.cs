//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using System.Net.Sockets;

namespace Utopia.Core.Net
{
    public class Socket : ISocket
    {
        System.Net.Sockets.Socket socket;

        public Socket(System.Net.Sockets.Socket socket)
        {
            ArgumentNullException.ThrowIfNull(socket, nameof(socket));
            this.socket = socket;
        }

        public void Close()
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }

        public void Flush()
        {

        }

        public async Task<int> Read(Memory<byte> output)
        {
            ArgumentNullException.ThrowIfNull(output, nameof(output));

            return await socket.ReceiveAsync(output, SocketFlags.None);
        }

        public async Task Write(Memory<byte> data, int start, int length)
        {
            ArgumentNullException.ThrowIfNull(data, nameof(data));

            var s = data.Slice(start, length);

            while (true)
            {
                var read = await socket.SendAsync(s, SocketFlags.None);

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
