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
    public class SocketHandle : IHandler
    {
        private readonly ISocket socket;
        private readonly Pipe pipe = new();
        private readonly PipeWriter writer;
        private readonly PipeReader reader;

        public const int PIPE_BUFFER_SIZE = 1024;

        public SocketHandle(ISocket socket)
        {
            ArgumentNullException.ThrowIfNull(socket, nameof(socket));
            this.socket = socket;
            this.writer = this.pipe.Writer;
            this.reader = this.pipe.Reader;
        }

        public async Task<object?> Write(IChannelContext ctx, object? input)
        {
            if (input == null)
            {
                socket.Flush();
            }
            else if (input is Memory<byte> m)
            {
                await socket.Write(m);
            }
            else
            {
                throw new ArgumentException("unknown type of output");
            }

            // we should has no next handler
            return null;
        }

        public async Task<object?> Read(IChannelContext ctx, object? input)
        {
            // write pipe
            var buffer = writer.GetMemory(PIPE_BUFFER_SIZE);
            var read = await socket.Read(buffer);
            writer.Advance(read);
            await writer.FlushAsync();

            // continue to next handle with a reader
            return reader;
        }

        public Task Disconnect(IChannelContext ctx) { return new Task(() => { socket.Close(); }); }

    }
}
