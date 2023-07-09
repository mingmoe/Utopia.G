//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//

namespace Utopia.Core;

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
    bool Cancel { set; get; }

    /// <summary>
    /// 事件是否能取消
    /// </summary>
    bool CanCancel { get; }
}

/// <summary>
/// 一个带有参数的事件
/// </summary>
/// <typeparam name="ParameterT">事件参数的类型</typeparam>
public interface IEventWithParam<ParameterT> :IEvent
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
