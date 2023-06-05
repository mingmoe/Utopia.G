using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        if (_handlers.TryGetValue(packetTypeId, out var handlers))
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

