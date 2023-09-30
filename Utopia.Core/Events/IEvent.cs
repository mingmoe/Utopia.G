#region copyright
// This file(may named IEvent.cs) is a part of the project: Utopia.Core.
// 
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// 
// This file is part of Utopia.Core.
//
// Utopia.Core is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Utopia.Core is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with Utopia.Core. If not, see <https://www.gnu.org/licenses/>.
#endregion

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
