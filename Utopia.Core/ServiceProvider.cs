//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utopia.Core
{
    public class ServiceProvider : IServiceProvider
    {
        readonly ConcurrentDictionary<Type, object> _services = new();

        public bool TryGetService<T>(out T? service)
        {
            if (this._services.TryGetValue(typeof(T), out object? value))
            {
                service = (T)value;
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
            if(this.TryGetService<T>(out T? obj))
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
            return _services.Values.ToArray();
        }

        public bool HasService<T>()
        {
            return _services.ContainsKey(typeof(T));
        }

        public void RemoveService<T>()
        {
            _services.Remove(typeof(T), out _);
        }

        public bool TryRegisterService<T>(T service)
        {
            ArgumentNullException.ThrowIfNull(service);

            return _services.TryAdd(typeof(T), service);
        }
    }
}
