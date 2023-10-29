// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

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
        lock (_locker)
        {
            _events.Add(handler);
        }
    }

    public void Unregister(Action<EventT> handler)
    {
        lock (_locker)
        {
            _ = _events.Remove(handler);
        }
    }

    public void ClearRegisters()
    {
        lock (_locker)
        {
            _events.Clear();
        }
    }

    /// <summary>
    /// 抛出事件。每个EventManager只允许同时存在一个事件链，其他线程的Fire调用将被堵塞。
    /// </summary>
    /// <param name="e">事件</param>
    public void Fire(EventT e)
    {
        Action<EventT>[] handlers;

        lock (_locker)
        {
            handlers = _events.ToArray();
        }

        lock (_fireLocker)
        {
            foreach (Action<EventT> handler in handlers)
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
