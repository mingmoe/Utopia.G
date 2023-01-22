//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Data;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utopia.Core.Net
{
    public class NativeSocket : ISocket
    {
        /// <summary>
        /// 创建本地套接字
        /// </summary>
        public static (ISocket server, ISocket client) Create()
        {
            Pipe server = new();
            Pipe client = new();

            // (read from client and write to server),(read from server and write to client)
            return new(new NativeSocket(client.Reader, server.Writer), new NativeSocket(server.Reader, client.Writer));
        }

        private readonly PipeReader reader;
        private readonly PipeWriter writer;

        private bool isConnected = true;

        private void UpdateStatus(bool compeleted)
        {
            if(!this.isConnected)
            {
                return;
            }
            if (compeleted)
            {
                this.isConnected = false;
            }
        }

        public string SocketAddress => "Native Socket";

        NativeSocket(PipeReader reader, PipeWriter writer)
        {
            ArgumentNullException.ThrowIfNull(reader);
            ArgumentNullException.ThrowIfNull(writer);

            this.reader = reader;
            this.writer = writer;
        }


        public void Close()
        {
            UpdateStatus(true);
            writer.Complete();
            reader.Complete();
        }

        public void Flush()
        {
            var result = writer.FlushAsync().AsTask();
            result.Wait();

            UpdateStatus(result.Result.IsCompleted);
        }

        public async Task<int> Read(Memory<byte> output)
        {
            var result = await reader.ReadAsync();
            var buffer = result.Buffer;

            UpdateStatus(result.IsCompleted);

            if (output.Length >= buffer.Length)
            {
                buffer.CopyTo(output.Span);
                reader.AdvanceTo(buffer.End);

                return (int)buffer.Length;
            }
            else
            {
                buffer = buffer.Slice(0, output.Length);
                buffer.CopyTo(output.Span);
                reader.AdvanceTo(buffer.Start, buffer.End);

                return output.Length;
            }
        }

        public async Task Write(Memory<byte> data, int start, int length)
        {
            var result = await writer.WriteAsync(data.Slice(start, length));
            var fresult = await writer.FlushAsync();

            UpdateStatus(result.IsCompleted);
            UpdateStatus(fresult.IsCompleted);
        }

        public bool Connected => isConnected;
    }
}
