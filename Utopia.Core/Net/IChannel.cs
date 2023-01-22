//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//

namespace Utopia.Core.Net
{
    /// <summary>
    /// 通道接口
    /// </summary>
    public interface IChannel
    {
        /// <summary>
        /// 发起write调用链
        /// </summary>
        /// <param name="data">要写入的数据</param>
        Task FireWrite(object? data);

        /// <summary>
        /// 从ChannelRoot中发起Read调用链
        /// </summary>
        Task FireRead();

        /// <summary>
        /// 发起Read调用链
        /// </summary>
        Task FireRead(object? data);

        /// <summary>
        /// 发起Connect调用链
        /// </summary>
        Task FireConnect();

        /// <summary>
        /// 发起Disconnect调用链
        /// </summary>
        Task FireDisconnect();

        /// <summary>
        /// 通道的标识符，人类可读
        /// </summary>
        string ChannelId { get; }
    }
}
