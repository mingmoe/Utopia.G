#region copyright
// This file(may named IVariableEvent.cs) is a part of the project: Utopia.Core.
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
/// 用于监控一个变量的事件,要求变量不可变.
/// 线程安全.
/// </summary>
public interface IVariableEvent<T>
{
    public T Value { get; set; }

    /// <summary>
    /// 一个可以取消的事件
    /// </summary>
    /// <typeparam name="VT"></typeparam>
    public class VariavbleChangedEvent<VT> : Event
    {
        public readonly VT Old;
        public readonly VT New;

        public VariavbleChangedEvent(VT old, VT @new)
        {
            this.Old = old;
            this.New = @new;
        }
    }

    /// <summary>
    /// 事件可取消
    /// </summary>
    public IEventManager<VariavbleChangedEvent<T>> Event { get; }
}

public class VariableEvent<T> : IVariableEvent<T>
{
    private readonly object _lock = new();
    private T _value;

    public VariableEvent(T defaultValue)
    {
        this._value = defaultValue;
    }

    public T Value
    {
        get
        {
            lock (this._lock)
            {
                return this._value;
            }
        }
        set
        {
            var @event = new IVariableEvent<T>.VariavbleChangedEvent<T>(this._value, value);
            this.Event.Fire(@event);
            lock (this._lock)
            {
                this._value = value;
            }
        }
    }

    public IEventManager<IVariableEvent<T>.VariavbleChangedEvent<T>> Event { get; init; }
    = new EventManager<IVariableEvent<T>.VariavbleChangedEvent<T>>();
}
