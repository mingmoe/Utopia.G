//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//

using System;
using System.Net.Sockets;

namespace Utopia.G;

/// <summary>
/// 客户端，负责连接到服务器
/// </summary>
public interface IClient
{
    /// <summary>
    /// 连接到服务器
    /// </summary>
    /// <param name="hostname">服务器地址</param>
    /// <param name="port">服务器端口</param>
    Socket Connect(string hostname, int port);

    /// <summary>
    /// 链接到服务器
    /// </summary>
    /// <param name="uri">服务器URL</param>
    Socket Connect(string uri)
    {
        var url = new Uri(uri);
        return this.Connect(url.Host, url.Port);
    }

    /// <summary>
    /// 结束链接，退出游戏。
    /// </summary>
    void Close();
}
