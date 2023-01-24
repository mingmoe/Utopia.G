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

namespace Utopia.Core.Net;

/// <summary>
/// 通道工厂
/// </summary>
public interface IChannelFactory
{
    /// <summary>
    /// 创建一个通道
    /// </summary>
    /// <param name="socket">套接字</param>
    /// <returns>通道</returns>
    IChannel Create(ISocket socket);
}
