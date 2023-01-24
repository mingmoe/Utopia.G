//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//

namespace Utopia.Core;

/// <summary>
/// 一个事件标准实现，非线程安全。
/// </summary>
public class Event : IEvent
{

    private bool _cancel;

    /// <summary>
    /// 构造一个事件
    /// </summary>
    /// <param name="param">事件参数</param>
    /// <param name="initResult">事件初始结果</param>
    /// <param name="cancelAble">事件能否被取消</param>
    public Event(object? param, object? initResult, bool cancelAble)
    {
        this.Parameter = param;
        this.Result = initResult;
        this.CanCancel = cancelAble;
    }

    /// <summary>
    /// 事件取消设置。
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// 如果事件无法被取消但此field仍被修改，则抛出此异常。
    /// </exception>
    public bool Cancel
    {
        set
        {
            if (this.CanCancel)
            {
                _cancel = true;
            }
            else
            {
                throw new InvalidOperationException("try to cancel a event that cannot be canceled");
            }
        }
        get
        {
            return _cancel;
        }
    }

    /// <summary>
    /// 事件是否能取消
    /// </summary>
    public bool CanCancel { get; init; }

    /// <summary>
    /// 事件参数
    /// </summary>
    public object? Parameter { get; init; }

    /// <summary>
    /// 事件返回值
    /// </summary>
    public object? Result { get; set; }
}
