//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Utopia.Core.Net;

namespace Utopia.G
{
    /// <summary>
    /// 负责连接到服务器
    /// </summary>
    public class Client : IClient
    {
        /// <summary>
        /// 链接到服务器
        /// </summary>
        /// <param name="host">服务器地址</param>
        /// <param name="port"></param>
        /// <exception cref="InvalidOperationException">该Client已经连接到服务器</exception>
        /// <exception cref="IOException">链接异常</exception>
        public ISocket Connect(string host, int port)
        {
            ArgumentNullException.ThrowIfNull(host);

            // Get host related information.
            IPHostEntry? hostEntry = Dns.GetHostEntry(host);

            System.Net.Sockets.Socket? tempSocket = null;

            foreach (IPAddress address in hostEntry.AddressList)
            {
                IPEndPoint ipe = new(address, port);
                tempSocket =
                    new(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                tempSocket.Connect(ipe);

                if (tempSocket.Connected)
                {
                    break;
                }
                else
                {
                    continue;
                }
            }

            if (tempSocket == null)
            {
                throw new IOException("failed to connect the server:" + host);
            }

            return new Utopia.Core.Net.Socket(tempSocket);
        }

    }
}
