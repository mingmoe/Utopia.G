//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using System.Collections.Concurrent;

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
        var arr = _services.ToArray();
        var list = new List<object>(arr.Length);

        foreach (var item in arr)
        {
            list.Add(item.Value);
        }

        return list.ToArray();
    }

    public bool HasService<T>()
    {
        return _services.ContainsKey(typeof(T));
    }

    public void RemoveService<T>()
    {
        var r = _services.TryRemove(typeof(T), out object? obj);

        // fire delete event
        if (r)
        {
            this.GetEventBusForService<T>().Fire(
                new ServiceChangedEvent<T>(ServiceChangedType.Delete, (T?)obj, true));
        }
    }

    public bool TryRegisterService<T>(T service)
    {
        ArgumentNullException.ThrowIfNull(service);

        var r = _services.TryAdd(typeof(T), service);

        // fire add event
        if (r)
        {
            this.GetEventBusForService<T>().Fire(
                new ServiceChangedEvent<T>(ServiceChangedType.Add, service, true));
        }

        return r;
    }

    public IEventManager<IServiceChangedEvent<T>> GetEventBusForService<T>()
    {
        return
            (IEventManager<IServiceChangedEvent<T>>)this._managers.GetOrAdd(typeof(T), (_) => new EventManager<IServiceChangedEvent<T>, ServiceChangedType, T>());
    }

    public bool TryUpdate<T>(T old, T @new)
    {
        ArgumentNullException.ThrowIfNull(@new);
        ArgumentNullException.ThrowIfNull(old);
        var t = this._services.TryUpdate(typeof(T), @new, old);

        // fire update event
        if (t)
        {
            this.GetEventBusForService<T>().Fire(
                new ServiceChangedEvent<T>(ServiceChangedType.Update, @new, true));
        }
        return t;
    }
}
