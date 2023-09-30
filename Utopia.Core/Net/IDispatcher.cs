#region copyright
// This file(may named IDispatcher.cs) is a part of the project: Utopia.Core.
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
using Utopia.Core.Utilities;

namespace Utopia.Core.Net;

/// <summary>
/// 负责对包进行分发，是线程安全的。
/// </summary>
public interface IDispatcher
{
    /// <summary>
    /// 如果这个包没有处理者，那么将返回false
    /// </summary>
    bool DispatchPacket(Guuid packetTypeId, object obj);

    void RegisterHandler(Guuid packetTypeId, Action<object> handler);

    void UnregisterHandler(Guuid packetTypeId, Action<object> handler);
}

public class Dispatcher : IDispatcher
{
    readonly SafeDictionary<Guuid, List<Action<object>>> _handlers = new();

    public bool DispatchPacket(Guuid packetTypeId, object obj)
    {
        bool handled = false;
        if (this._handlers.TryGetValue(packetTypeId, out var handlers))
        {
            foreach (var handler in handlers!)
            {
                handled = true;
                handler.Invoke(obj);
            }
        }
        return handled;
    }

    public void RegisterHandler(Guuid packetTypeId, Action<object> handler)
    {
        this._handlers.AddOrUpdate(packetTypeId,
            (id) => { var l = new List<Action<object>> { handler }; return l; },
            (id, list) => { list.Add(handler); return list; });
    }

    public void UnregisterHandler(Guuid packetTypeId, Action<object> handler)
    {
        this._handlers.TryRemove(packetTypeId, out var _);
    }
}

