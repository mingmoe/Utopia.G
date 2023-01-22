//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//

namespace Utopia.Core.Net
{
    public class ChannelContext : IChannelContext
    {
        readonly IList<IHandler> handlers;

        /// <summary>
        /// 我们将忽略第一次NextHandle()调用。
        /// </summary>
        bool begin = false;

        int ptr = 0;

        public ChannelContext(IChannel channel, IList<IHandler> handlers)
        {
            this.Channel = channel;
            this.handlers = handlers;
        }

        public IChannel Channel { get; init; }

        public IHandler Current
        {
            get
            {
                return handlers[ptr];
            }
        }

        public bool HasNext()
        {
            return handlers.Count == 0 || ptr < (handlers.Count - 1);
        }

        public bool HasPrev()
        {
            return handlers.Count != 0;
        }

        public void NextHandle()
        {
            if (begin)
            {
                ptr++;
            }
            else
            {
                begin = true;
            }
        }

        public void PrevHandle()
        {
            if (ptr != 0)
                ptr--;
        }

        public async Task Write(object? obj)
        {
            var ctx = new ChannelContext(this.Channel, this.handlers)
            {
                ptr = this.ptr
            };

            while (ctx.ptr != 0)
            {
                ctx.PrevHandle();
                obj = await ctx.Current.Write(ctx, obj);
            }
        }
    }
}
