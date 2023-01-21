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
    /// 通道上下文
    /// </summary>
    public interface IChannelContext
    {
        /// <summary>
        /// 这个函数专门为Read调用链中写入而准备。
        /// </summary>
        /// <returns></returns>
        Task Write(object? obj);

        /// <summary>
        /// 当前所处的通道
        /// </summary>
        IChannel Channel { get; }

        /// <summary>
        /// 移动到下一个handle。第一次调用将忽略。
        /// </summary>
        void NextHandle();

        /// <summary>
        /// 获取当前Handler
        /// </summary>
        IHandler Current { get; }

        /// <summary>
        /// 检查是否还有下一个handle
        /// </summary>
        bool HasNext();
    }
}
