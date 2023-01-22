//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using System.IO.Pipelines;

namespace Utopia.Core.Net
{
    /// <summary>
    /// 套接字接口
    /// </summary>
    public class SocketChannelRoot : IChannelRoot
    {
        public ISocket Socket { get; private set; }

        public const int READ_BUFFER_SIZE = 1024;

        public SocketChannelRoot(ISocket socket)
        {
            ArgumentNullException.ThrowIfNull(socket, nameof(socket));
            this.Socket = socket;
        }

        public async Task Write(object? input)
        {
            if (input == null)
            {
                Socket.Flush();
            }
            else if (input is Memory<byte> m)
            {
                await Socket.Write(m);
            }
            else if (input is ReadOnlyMemory<byte> rm)
            {
                var mem = new Memory<byte>(new byte[rm.Length]);
                rm.CopyTo(mem);
                await Socket.Write(mem);
            }
            else if (input is byte[] b)
            {
                await Socket.Write(b);
            }
            else if(input is ArraySegment<byte> a)
            {
                await Socket.Write(a);
            }
            else if(input is IEnumerable<byte> be)
            {
                await Socket.Write(be.ToArray());
            }
            else
            {
                throw new ArgumentException("unknown type of output");
            }
        }

        public async Task<object?> Read()
        {
            var buf = new byte[READ_BUFFER_SIZE];

            var size = await Socket.Read(buf);

            return buf.AsMemory(0)[..size];
        }
    }
}
