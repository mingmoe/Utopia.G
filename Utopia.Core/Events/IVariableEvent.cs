// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

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
            Old = old;
            New = @new;
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

    public VariableEvent(T defaultValue) => _value = defaultValue;

    public T Value
    {
        get
        {
            lock (_lock)
            {
                return _value;
            }
        }
        set
        {
            var @event = new IVariableEvent<T>.VariavbleChangedEvent<T>(_value, value);
            Event.Fire(@event);
            lock (_lock)
            {
                _value = value;
            }
        }
    }

    public IEventManager<IVariableEvent<T>.VariavbleChangedEvent<T>> Event { get; init; }
    = new EventManager<IVariableEvent<T>.VariavbleChangedEvent<T>>();
}
