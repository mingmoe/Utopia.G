//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//

using System.Xml.XPath;

namespace Utopia.Core;

public class Event : IEvent
{
    private bool _cancel;

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

    public Event(bool cancelable)
    {
        this.CanCancel = cancelable;
    }
}

public class EventWithParam<ParamT> : Event, IEventWithParam<ParamT>
{
    public ParamT? Parameter { get; set; }

    public EventWithParam(ParamT? parameter, bool cancelable) : base(cancelable)
    {
        this.Parameter = parameter;
    }
}

public class EventWithResult<ResultT> : Event, IEventWithResult<ResultT>
{
    public ResultT? Result { get; set; }

    public EventWithResult(ResultT? result, bool cancelable) : base(cancelable)
    {
        this.Result = result;
    }
}

/// <summary>
/// 一个事件标准实现，非线程安全。
/// </summary>
public class ComplexEvent<ParameterT, RustleT> :
    EventWithParam<ParameterT>,
    IEventWithParamAndResult<ParameterT, RustleT>
{
    /// <summary>
    /// 构造一个事件
    /// </summary>
    /// <param name="param">事件参数</param>
    /// <param name="initResult">事件初始结果</param>
    /// <param name="cancelAble">事件能否被取消</param>
    public ComplexEvent(ParameterT? param, RustleT? initResult, bool cancelable) : base(param, cancelable)
    {
        this.Result = initResult;
    }

    /// <summary>
    /// 事件返回值
    /// </summary>
    public RustleT? Result { get; set; }
}
