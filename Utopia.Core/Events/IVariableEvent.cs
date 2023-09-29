using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public VariavbleChangedEvent(VT old, VT @new) : base(true)
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
            lock (_lock)
            {
                return this._value;
            }
        }
        set
        {
            var @event = new IVariableEvent<T>.VariavbleChangedEvent<T>(this._value, value);
            this.Event.Fire(@event);
            lock (_lock)
            {
                this._value = value;
            }
        }
    }

    public IEventManager<IVariableEvent<T>.VariavbleChangedEvent<T>> Event { get; init; }
    = new EventManager<IVariableEvent<T>.VariavbleChangedEvent<T>>();
}
