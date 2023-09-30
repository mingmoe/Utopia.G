#region copyright
// This file(may named EventManager.cs) is a part of the project: Utopia.Core.
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

namespace Utopia.Core.Events;

/// <summary>
/// 事件管理器，负责管理一类事件，线程安全。
/// </summary>
public class EventManager<EventT> : IEventManager<EventT> where EventT : IEvent
{

    private readonly List<Action<EventT>> _events = new();

    /// <summary>
    /// lock when modify the event handlers list
    /// </summary>
    private readonly object _locker = new();

    /// <summary>
    /// lock when fire event
    /// </summary>
    private readonly object _fireLocker = new();

    public EventManager() { }

    /// <summary>
    /// 注册事件
    /// </summary>
    /// <param name="handler">事件处理者</param>
    public void Register(Action<EventT> handler)
    {
        lock (this._locker)
        {
            this._events.Add(handler);
        }
    }

    public void Unregister(Action<EventT> handler)
    {
        lock (this._locker)
        {
            this._events.Remove(handler);
        }
    }

    public void ClearRegisters()
    {
        lock (this._locker)
        {
            this._events.Clear();
        }
    }

    /// <summary>
    /// 抛出事件。每个EventManager只允许同时存在一个事件链，其他线程的Fire调用将被堵塞。
    /// </summary>
    /// <param name="e">事件</param>
    public void Fire(EventT e)
    {
        Action<EventT>[] handlers;

        lock (this._locker)
        {
            handlers = this._events.ToArray();
        }

        lock (this._fireLocker)
        {
            foreach (var handler in handlers)
            {
                if (e.Cancel)
                {
                    break;
                }
                handler.Invoke(e);
            }
        }
    }
}
