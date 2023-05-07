//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//

using System.Net.Sockets;

namespace Utopia.Server;

/// <summary>
/// 网络服务器接口，线程安全。
/// </summary>
public interface INetServer
{
    /// <summary>
    /// 链接并监听端口，一个INetServer只能监听一个端口。
    /// </summary>
    /// <param name="port">端口号</param>
    void Listen(int port);

    /// <summary>
    /// 接受新链接，必须监听一个端口号后才能调用。
    /// </summary>
    /// <returns></returns>
    Task<Socket> Accept();

    /// <summary>
    /// 停机，释放资源。
    /// </summary>
    void Shutdown();
}
