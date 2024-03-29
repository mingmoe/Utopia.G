// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using Utopia.Core.Exceptions;

namespace Utopia.Core.Events;

/// <summary>
/// The base event implement
/// </summary>
public class Event : IEvent
{
    private volatile bool _canceled = false;
    public bool Cancel
    {
        get => _canceled;
        protected set => _canceled = value;
    }

    /// <summary>
    /// Get the retsult of the event, or throw a <see cref="EventAssertionException"/>
    /// with information.
    /// </summary>
    public static T GetResult<T>(IEventWithResult<T> e) => e.Result == null ? throw new EventAssertionException(EventAssertionFailedCode.ResultIsNull) : e.Result;
}

public class CancelableEvent : Event, IEvent, ICancelable
{
    public void SetCancel(bool isCancel) => Cancel = isCancel;
}

public class EventWithParam<ParamT>(ParamT? parameter) : Event(), IEventWithParam<ParamT>
{
    public ParamT? Parameter { get; set; } = parameter;
}

public class CancelableEventWithParam<ParamT>(ParamT? parameter) : CancelableEvent(), IEventWithParam<ParamT>
{
    public ParamT? Parameter { get; set; } = parameter;
}

public class EventWithResult<ResultT>(ResultT? result) : Event(), IEventWithResult<ResultT>
{
    public ResultT? Result { get; set; } = result;
}

public class CancelableEventWithResult<ResultT>(ResultT? result) : CancelableEvent(), IEventWithResult<ResultT>
{
    public ResultT? Result { get; set; } = result;
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
    public ComplexEvent(ParameterT? param, RustleT? initResult) : base(param) => Result = initResult;

    /// <summary>
    /// 事件返回值
    /// </summary>
    public RustleT? Result { get; set; }
}

/// <summary>
/// 一个事件标准实现，非线程安全。
/// </summary>
public class CancelableComplexEvent<ParameterT, RustleT> :
CancelableEventWithParam<ParameterT>,
IEventWithParamAndResult<ParameterT, RustleT>
{
    /// <summary>
    /// 构造一个事件
    /// </summary>
    /// <param name="param">事件参数</param>
    /// <param name="initResult">事件初始结果</param>
    /// <param name="cancelAble">事件能否被取消</param>
    public CancelableComplexEvent(ParameterT? param, RustleT? initResult) : base(param) => Result = initResult;

    /// <summary>
    /// 事件返回值
    /// </summary>
    public RustleT? Result { get; set; }
}

