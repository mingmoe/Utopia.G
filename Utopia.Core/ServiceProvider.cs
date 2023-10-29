// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using Utopia.Core.Collections;
using Utopia.Core.Events;

namespace Utopia.Core;

public class ServiceProvider : IServiceProvider
{
    private readonly SafeDictionary<Type, object> _services = new();
    private readonly SafeDictionary<Type, object> _managers = new();

    public bool TryGetService<T>(out T? service)
    {
        if (_services.TryGetValue(typeof(T), out object? value))
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

    public T GetService<T>() => TryGetService(out T? obj) ? obj! : throw new InvalidOperationException("the type of the service has not beed registered");

    public IReadOnlyCollection<object> GetServices()
    {
        KeyValuePair<Type, object>[] arr = _services.ToArray();
        var list = new List<object>(arr.Length);

        foreach (KeyValuePair<Type, object> item in arr)
        {
            list.Add(item.Value);
        }

        return list.ToArray();
    }

    public bool HasService<T>() => _services.ContainsKey(typeof(T));

    public void RemoveService<T>()
    {
        bool r = _services.TryRemove(typeof(T), out object? obj);

        // fire delete event
        if (r)
        {
            GetEventBusForService<T>().Fire(
                new ServiceChangedEvent<T>(ServiceChangedType.Delete, (T)obj!));
        }
    }

    public bool TryRegisterService<T>(T service)
    {
        ArgumentNullException.ThrowIfNull(service);

        bool r = _services.TryAdd(typeof(T), service);

        // fire add event
        if (r)
        {
            GetEventBusForService<T>().Fire(
                new ServiceChangedEvent<T>(ServiceChangedType.Add, service));
        }

        return r;
    }

    public IEventManager<IServiceChangedEvent<T>> GetEventBusForService<T>() => (IEventManager<IServiceChangedEvent<T>>)_managers.GetOrAdd(typeof(T), (_) => new EventManager<IServiceChangedEvent<T>>());

    public bool TryUpdate<T>(T old, T @new)
    {
        ArgumentNullException.ThrowIfNull(@new);
        ArgumentNullException.ThrowIfNull(old);
        bool ret = false;

        _ = _services.AddOrUpdate(typeof(T), (key) =>
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
            GetEventBusForService<T>().Fire(e);

            return e.Target == null ? throw new InvalidOperationException("for the update event,the Target is null") : (object)e.Target;
        });

        return ret;
    }

    public void Dispose()
    {
        var list = new List<IDisposable>();

        list.AddRange((IEnumerable<IDisposable>)_services.ToArray().TakeWhile((pair) =>
        {
            return pair.Value is IDisposable;
        }));

        list.AddRange((IEnumerable<IDisposable>)_managers.ToArray().TakeWhile((pair) =>
        {
            return pair.Value is IDisposable;
        }));

        foreach (IDisposable item in list)
        {
            item.Dispose();
        }
        GC.SuppressFinalize(this);
    }
}
