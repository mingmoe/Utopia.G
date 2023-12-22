// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

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
    private readonly SafeDictionary<Guuid, List<Action<object>>> _handlers = new();

    public bool DispatchPacket(Guuid packetTypeId, object obj)
    {
        bool handled = false;
        if (_handlers.TryGetValue(packetTypeId, out List<Action<object>>? handlers))
        {
            foreach (Action<object> handler in handlers!)
            {
                handled = true;
                handler.Invoke(obj);
            }
        }
        return handled;
    }

    public void RegisterHandler(Guuid packetTypeId, Action<object> handler) => _handlers.AddOrUpdate(packetTypeId,
            (id) => { var l = new List<Action<object>> { handler }; return l; },
            (id, list) => { list.Add(handler); return list; });

    public void UnregisterHandler(Guuid packetTypeId, Action<object> handler)
    {
        if(_handlers.TryGetValue(packetTypeId,out var list)){
            list.Remove(handler);
        }

        return;
    }
}

