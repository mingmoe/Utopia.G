// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using Utopia.Core.Exceptions;

namespace Utopia.Core.Events;

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
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="UT"></typeparam>
    /// <param name="e"></param>
    /// <returns></returns>
    /// <exception cref="EventAssertionException"></exception>
    public static UT GetResult<T, UT>(T e) where T : IEventWithResult<UT> => e.Result == null ? throw new EventAssertionException(EventAssertionFailedCode.ResultIsNull) : e.Result;
}

public class CancelableEvent : Event, IEvent, ICancelable
{
    public void SetCancel(bool isCancel) => Cancel = isCancel;
}

public class EventWithParam<ParamT> : Event, IEventWithParam<ParamT>
{
    public ParamT? Parameter { get; set; }

    public EventWithParam(ParamT? parameter) : base() => Parameter = parameter;
}
public class CancelableEventWithParam<ParamT> : CancelableEvent, IEventWithParam<ParamT>
{
    public ParamT? Parameter { get; set; }

    public CancelableEventWithParam(ParamT? parameter) : base() => Parameter = parameter;
}

public class EventWithResult<ResultT> : Event, IEventWithResult<ResultT>
{
    public ResultT? Result { get; set; }

    public EventWithResult(ResultT? result) : base() => Result = result;
}

public class CancelableEventWithResult<ResultT> : CancelableEvent, IEventWithResult<ResultT>
{
    public ResultT? Result { get; set; }

    public CancelableEventWithResult(ResultT? result) : base() => Result = result;
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

