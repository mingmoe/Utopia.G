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
    public class VariableChangedEvent : Event
    {
        public readonly T Old;
        public readonly T New;

        public VariableChangedEvent(T old, T @new)
        {
            Old = old;
            New = @new;
        }
    }

    public IEventManager<VariableChangedEvent> Event { get; }
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
            var @event = new IVariableEvent<T>.VariableChangedEvent(_value, value);
            Event.Fire(@event);
            lock (_lock)
            {
                _value = value;
            }
        }
    }

    public IEventManager<IVariableEvent<T>.VariableChangedEvent> Event { get; init; }
    = new EventManager<IVariableEvent<T>.VariableChangedEvent>();
}
