//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using System.Net;
using System.Net.Sockets;

namespace Utopia.Server
{
    /// <summary>
    /// 网络服务器，此类非线程安全
    /// </summary>
    public class NetServer
    {

        public Socket? Socket = null;

        /// <summary>
        /// 监听端口
        /// </summary>
        /// <param name="port">端口</param>
        /// <exception cref="InvalidOperationException">该服务器已经监听了某个端口，并且未停机。</exception>
        public void Listen(int port)
        {
            if (Socket != null)
            {
                throw new InvalidOperationException("the server has listened and it's not closed");
            }

            // create the socket
            Socket = new Socket(AddressFamily.InterNetwork,
                                             SocketType.Stream,
                                             ProtocolType.Tcp);

            // bind the listening socket to the port
            IPAddress hostIP = Dns.GetHostEntry(IPAddress.Any.ToString()).AddressList[0];
            IPEndPoint ep = new(hostIP, port);
            Socket.Bind(ep);

            // start listening
            Socket.Listen(128);
        }

        public async Task<Socket> Accept()
        {
            if (Socket == null)
            {
                throw new InvalidOperationException("the server hasn't listened");
            }

            return await Socket.AcceptAsync();
        }

        /// <summary>
        /// 停机，释放资源
        /// </summary>
        public void Shutdown()
        {
            if (Socket != null)
            {
                Socket.Shutdown(SocketShutdown.Both);
                Socket.Close();
                Socket.Dispose();
            }
            Socket = null;
        }
    }
}
