//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using Castle.Core.Logging;
using Castle.Services.Logging.NLogIntegration;
using System.Collections.Concurrent;

namespace Utopia.Core.Logging;

/// <summary>
/// 使用缓存的日志工厂，对于使用相同的name调用，将会返回相同的Logger。
/// 对于<see cref="GetLogger(object?)"/>和<see cref="GetLogger(Type)"/>，
/// 将会调用(<see cref="object.GetType()"/>).<see cref="object.ToString()"/>转化为字符串。
/// 对于空的object，name将为"null"。
/// </summary>
public class CacheLoggerFactory : NLogFactory
{
    readonly ConcurrentDictionary<string, ILogger> _cached = new();

    public ILogger GetLogger(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        return this.GetLogger(type.FullName ?? type.Name);
    }

    public ILogger GetLogger(object? obj)
    {
        if (obj is null)
        {
            return this.GetLogger("null");
        }
        return this.GetLogger(obj.GetType());
    }

    public ILogger GetLogger(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        return _cached.GetOrAdd(name, (name) =>
        {
            return new NLogLogger(NLog.LogManager.GetLogger(name), this);
        });
    }
}
