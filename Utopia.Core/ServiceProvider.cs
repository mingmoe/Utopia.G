#region copyright
// This file(may named ServiceProvider.cs) is a part of the project: Utopia.Core.
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
using Utopia.Core.Events;

namespace Utopia.Core;

public class ServiceProvider : IServiceProvider
{
    readonly SafeDictionary<Type, object> _services = new();

    readonly SafeDictionary<Type, object> _managers = new();

    public bool TryGetService<T>(out T? service)
    {
        if (this._services.TryGetValue(typeof(T), out var value))
        {
            service = (T?)value;
            return true;
        }
        else
        {
            service = default;
            return false;
        }
    }

    public T GetService<T>()
    {
        if (this.TryGetService<T>(out T? obj))
        {
            return obj!;
        }
        else
        {
            throw new InvalidOperationException("the type of the service has not beed registered");
        }
    }

    public IReadOnlyCollection<object> GetServices()
    {
        var arr = this._services.ToArray();
        var list = new List<object>(arr.Length);

        foreach (var item in arr)
        {
            list.Add(item.Value);
        }

        return list.ToArray();
    }

    public bool HasService<T>()
    {
        return this._services.ContainsKey(typeof(T));
    }

    public void RemoveService<T>()
    {
        var r = this._services.TryRemove(typeof(T), out object? obj);

        // fire delete event
        if (r)
        {
            this.GetEventBusForService<T>().Fire(
                new ServiceChangedEvent<T>(ServiceChangedType.Delete, (T)obj!));
        }
    }

    public bool TryRegisterService<T>(T service)
    {
        ArgumentNullException.ThrowIfNull(service);

        var r = this._services.TryAdd(typeof(T), service);

        // fire add event
        if (r)
        {
            this.GetEventBusForService<T>().Fire(
                new ServiceChangedEvent<T>(ServiceChangedType.Add, service));
        }

        return r;
    }

    public IEventManager<IServiceChangedEvent<T>> GetEventBusForService<T>()
    {
        return
            (IEventManager<IServiceChangedEvent<T>>)this._managers.GetOrAdd(typeof(T), (_) => new EventManager<IServiceChangedEvent<T>>());
    }

    public bool TryUpdate<T>(T old, T @new)
    {
        ArgumentNullException.ThrowIfNull(@new);
        ArgumentNullException.ThrowIfNull(old);
        bool ret = false;

        this._services.AddOrUpdate(typeof(T), (key) =>
        {
            return key;
        },
        (key, exist) =>
        {
            if (!ReferenceEquals(old, @new))
            {
                return exist;
            }
            ret = true;

            var e =
                new ServiceChangedEvent<T>(ServiceChangedType.Update, @new)
                {
                    Old = (T)exist,
                };
            this.GetEventBusForService<T>().Fire(e);

            if (e.Target == null)
            {
                throw new InvalidOperationException("for the update event,the Target is null");
            }

            return e.Target;
        });

        return ret;
    }

    public void Dispose()
    {
        var list = new List<IDisposable>();

        list.AddRange((IEnumerable<IDisposable>)this._services.ToArray().TakeWhile((pair) =>
        {
            return (pair.Value is IDisposable);
        }));

        list.AddRange((IEnumerable<IDisposable>)this._managers.ToArray().TakeWhile((pair) =>
        {
            return (pair.Value is IDisposable);
        }));

        foreach(var item in list)
        {
            item.Dispose();
        }
        GC.SuppressFinalize(this);
    }
}
