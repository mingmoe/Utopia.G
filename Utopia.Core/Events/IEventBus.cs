// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using Utopia.Core.Collections;

namespace Utopia.Core.Events;

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
    private readonly object _lock = new();
    private readonly SafeDictionary<Type, object> _handlers = new();

    private List<Action<T>> _Get<T>() => (List<Action<T>>)_handlers.GetOrAdd
            (typeof(T), (t) => { return new List<Action<T>>(); });

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
            _handlers.Clear();
        }
    }

    public void Fire<T>(T @event)
    {
        lock (_lock)
        {
            List<Action<T>> handlers = _Get<T>();

            foreach (Action<T> handle in handlers)
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
            _ = _Get<T>().Remove(handler);
        }
    }
}
