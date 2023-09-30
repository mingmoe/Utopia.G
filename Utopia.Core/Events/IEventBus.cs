#region copyright
// This file(may named IEventBus.cs) is a part of the project: Utopia.Core.
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
    readonly object _lock = new();

    readonly SafeDictionary<Type, object> _handlers = new();

    List<Action<T>> _Get<T>()
    {
        return (List<Action<T>>)this._handlers.GetOrAdd
            (typeof(T), (t) => { return new List<Action<T>>(); });
    }

    public void Clear<T>()
    {
        lock (this._lock)
        {
            this._Get<T>().Clear();
        }
    }

    public void ClearAll()
    {
        lock (this._lock)
        {
            this._handlers.Clear();
        }
    }

    public void Fire<T>(T @event)
    {
        lock (this._lock)
        {
            var handlers = this._Get<T>();

            foreach (var handle in handlers)
            {
                handle.Invoke(@event);
            }
        }
    }

    public void Register<T>(Action<T> handler)
    {
        lock (this._lock)
        {
            this._Get<T>().Add(handler);
        }
    }

    public void Unregister<T>(Action<T> handler)
    {
        lock (this._lock)
        {
            this._Get<T>().Remove(handler);
        }
    }
}
