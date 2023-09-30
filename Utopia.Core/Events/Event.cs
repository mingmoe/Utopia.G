#region copyright
// This file(may named Event.cs) is a part of the project: Utopia.Core.
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

using Utopia.Core.Exceptions;

namespace Utopia.Core.Events;

public class Event : IEvent
{
    private bool _cancel;

    public bool Cancel
    {
        set
        {
            if (this.CanCancel)
            {
                this._cancel = true;
            }
            else
            {
                throw new EventAssertionException(EventAssertionFailedCode.EventCancled);
            }
        }
        get
        {
            return this._cancel;
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
