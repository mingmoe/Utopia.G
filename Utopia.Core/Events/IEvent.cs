// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

namespace Utopia.Core.Events;

/// <summary>
/// 代表一个标准事件
/// </summary>
public interface IEvent
{

    /// <summary>
    /// 事件取消设置。
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// 如果事件无法被取消但此field仍被修改，则抛出此异常。
    /// </exception>
    bool Cancel { get; }

}

/// <summary>
/// 一个带有参数的事件
/// </summary>
/// <typeparam name="ParameterT">事件参数的类型</typeparam>
public interface IEventWithParam<ParameterT> : IEvent
{
    /// <summary>
    /// 事件参数
    /// </summary>
    ParameterT? Parameter { get; }
}

/// <summary>
/// 一个带有输出的事件
/// </summary>
/// <typeparam name="RustleT"></typeparam>
public interface IEventWithResult<RustleT> : IEvent
{
    /// <summary>
    /// 事件返回值
    /// </summary>
    RustleT? Result { get; set; }
}

/// <summary>
/// 代表一个事件接口，带有参数和输出
/// </summary>
public interface IEventWithParamAndResult<ParameterT, RustleT> : IEvent,
    IEventWithParam<ParameterT>, IEventWithResult<RustleT>
{
}
