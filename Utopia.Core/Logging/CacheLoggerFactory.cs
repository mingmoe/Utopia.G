#region copyright
// This file(may named CacheLoggerFactory.cs) is a part of the project: Utopia.Core.
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

        return this._cached.GetOrAdd(name, (name) =>
        {
            return new NLogLogger(NLog.LogManager.GetLogger(name), this);
        });
    }
}
