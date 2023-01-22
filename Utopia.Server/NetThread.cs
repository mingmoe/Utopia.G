//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utopia.Core.Net;

namespace Utopia.Server
{
    /// <summary>
    /// 网络线程
    /// </summary>
    public class NetThread
    {
        readonly object locker = new();
        bool shutdown = true;
        readonly System.Net.Sockets.Socket socket;
        readonly ChannelFactory factory;
        readonly List<ChannelControlr> controllers = new();

        public NetThread(System.Net.Sockets.Socket serverSocket, ChannelFactory factory)
        {
            ArgumentNullException.ThrowIfNull(serverSocket);
            ArgumentNullException.ThrowIfNull(factory);
            this.socket = serverSocket;
            this.factory = factory;
            Thread.CurrentThread.Name = "Server Net Manager";
        }

        public void Shutdown()
        {
            shutdown = true;
            lock (locker)
            {
                foreach(var c in controllers)
                {
                    c.StopFireLoopAndDisconnect();
                }
                controllers.Clear();
            }
        }

        public void Run()
        {
            shutdown = false;

            while (!shutdown)
            {
                var s = socket.Accept();
                var ss = new Socket(s);
                var channel = factory.Create(ss);

                var controlr = new ChannelControlr(channel, ss);

                lock (locker)
                {
                    if (shutdown)
                    {
                        ss.Close();
                        return;
                    }
                    controllers.Add(controlr);
                    controlr.FireLoop();
                }
            }
        }
    }
}
