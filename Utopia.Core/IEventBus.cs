using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utopia.Core;

/// <summary>
/// 事件总线
/// </summary>
public interface IEventBus
{
    // TODO
    public void Register<T>(Action<T> handler);

    public void Unregister<T>(Action<T> handler);

    public void Clear<T>();

    public void ClearAll();

    public void Fire<T>(T @event);
}

public class EventBus : IEventBus
{
    readonly object _lock = new();

    SafeDictionary<Type, object> _handlers = new();

    List<Action<T>> _Get<T>()
    {
        return (List<Action<T>>)this._handlers.GetOrAdd
            (typeof(T), (t) => { return new List<Action<T>>(); });
    }

    public void Clear<T>()
    {
        lock (_lock)
        {
            _Get<T>().Clear();
        }
    }

    public void ClearAll()
    {
        lock (_lock)
        {
            this._handlers.Clear();
        }
    }

    public void Fire<T>(T @event)
    {
        lock (_lock)
        {
            var handlers = _Get<T>();

            foreach (var handle in handlers)
            {
                handle.Invoke(@event);
            }
        }
    }

    public void Register<T>(Action<T> handler)
    {
        lock (_lock)
        {
            _Get<T>().Add(handler);
        }
    }

    public void Unregister<T>(Action<T> handler)
    {
        lock (_lock)
        {
            _Get<T>().Remove(handler);
        }
    }
}
