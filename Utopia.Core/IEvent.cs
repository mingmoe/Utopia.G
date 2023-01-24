//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//

namespace Utopia.Core;

/// <summary>
/// 代表一个事件接口
/// </summary>
public interface IEvent
{
    /// <summary>
    /// 事件取消设置。
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// 如果事件无法被取消但此field仍被修改，则抛出此异常。
    /// </exception>
    bool Cancel { set; get; }

    /// <summary>
    /// 事件是否能取消
    /// </summary>
    bool CanCancel { get; }

    /// <summary>
    /// 事件参数
    /// </summary>
    object? Parameter { get; }

    /// <summary>
    /// 事件返回值
    /// </summary>
    object? Result { get; set; }
}
