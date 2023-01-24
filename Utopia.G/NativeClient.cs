//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utopia.Core.Net;

namespace Utopia.G
{
    /// <summary>
    /// 本地服务器
    /// </summary>
    public class NativeClient : IClient
    {
        private ISocket? _socket = null;

        public ISocket? Server
        {
            get { return this._socket; }
        }

        public ISocket Connect(string hostname, int port)
        {
            var (server, client) = NativeSocket.Create();

            this._socket = server;

            return client;
        }
    }
}
