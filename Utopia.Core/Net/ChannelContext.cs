//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//

namespace Utopia.Core.Net;

public class ChannelContext : IChannelContext
{
    readonly IList<IHandler> _handlers;

    /// <summary>
    /// 我们将忽略第一次NextHandle()调用。
    /// </summary>
    bool _begin = false;

    int _ptr = 0;

    public ChannelContext(IChannel channel, IList<IHandler> handlers)
    {
        this.Channel = channel;
        this._handlers = handlers;
    }

    public IChannel Channel { get; init; }

    public IHandler Current
    {
        get
        {
            return _handlers[_ptr];
        }
    }

    public bool HasNext()
    {
        return (_ptr + 1) < _handlers.Count;
    }

    public bool HasPrev()
    {
        return _handlers.Count != 0;
    }

    public void NextHandle()
    {
        if (_begin)
        {
            _ptr++;
        }
        else
        {
            _begin = true;
        }
    }

    public void PrevHandle()
    {
        if (_ptr != 0)
        {
            _ptr--;
        }
    }

    public async Task Write(object? obj)
    {
        var ctx = new ChannelContext(this.Channel, this._handlers)
        {
            _ptr = this._ptr
        };

        while (ctx._ptr != 0)
        {
            ctx.PrevHandle();
            obj = await ctx.Current.Write(ctx, obj);
        }

        await this.Channel.Root.Write(obj);
    }
}
