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

namespace Utopia.Server;

/// <summary>
/// 网络线程
/// </summary>
public class NetThread
{
    readonly object _locker = new();
    bool _shutdown = true;
    readonly System.Net.Sockets.Socket _socket;
    readonly IChannelFactory _factory;
    readonly List<ChannelControlr> _controllers = new();

    public NetThread(System.Net.Sockets.Socket serverSocket, IChannelFactory factory)
    {
        ArgumentNullException.ThrowIfNull(serverSocket);
        ArgumentNullException.ThrowIfNull(factory);
        this._socket = serverSocket;
        this._factory = factory;
        Thread.CurrentThread.Name = "Server Net Manager";
    }

    public void Shutdown()
    {
        _shutdown = true;
        lock (_locker)
        {
            foreach(var c in _controllers)
            {
                c.StopFireLoopAndDisconnect();
            }
            _controllers.Clear();
        }
    }

    public void Run()
    {
        _shutdown = false;

        while (!_shutdown)
        {
            var s = _socket.Accept();
            var ss = new Socket(s);
            var channel = _factory.Create(ss);

            var controlr = new ChannelControlr(channel, ss);

            lock (_locker)
            {
                if (_shutdown)
                {
                    ss.Close();
                    return;
                }
                _controllers.Add(controlr);
                controlr.FireLoop();
            }
        }
    }
}
