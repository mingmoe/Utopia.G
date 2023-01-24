//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//

using System.Threading.Tasks;
using Utopia.Core.Net;

namespace Utopia.G;

/// <summary>
/// 客户端
/// </summary>
public interface IClient
{
    /// <summary>
    /// 连接到服务器
    /// </summary>
    /// <param name="hostname">服务器地质</param>
    /// <param name="port">服务器端口</param>
    ISocket Connect(string hostname, int port);
}
