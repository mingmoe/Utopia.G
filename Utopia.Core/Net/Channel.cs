//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//

namespace Utopia.Core.Net;

/// <summary>
/// 通道实现
/// </summary>
public class Channel : IChannel
{

    public List<IHandler> Handlers { get; } = new List<IHandler>();

    public string ChannelId { get; set; } = "Channel";

    public IChannelRoot Root { get; init; }

    public Channel(IChannelRoot root)
    {
        this.Root = root;
    }

    public async Task FireRead()
    {
        // get data from the root
        await this.FireRead(await Root.Read());
    }

    public async Task FireRead(object? data)
    {
        var ctx = new ChannelContext(this, this.Handlers);
        do
        {
            ctx.NextHandle();
            data = await ctx.Current.Read(ctx, data);
        } while (ctx.HasNext());
    }

    public async Task FireWrite(object? data)
    {
        // 倒序
        var t = new List<IHandler>(this.Handlers);
        t.Reverse();
        var ctx = new ChannelContext(this, t);
        do
        {
            ctx.NextHandle();
            data = await ctx.Current.Write(ctx, data);
        } while (ctx.HasNext());

        // write the data at last
        await Root.Write(data);
    }

    public async Task FireConnect()
    {
        var ctx = new ChannelContext(this, this.Handlers);
        do
        {
            ctx.NextHandle();
            await ctx.Current.Connect(ctx);
        } while (ctx.HasNext());
    }

    public async Task FireDisconnect()
    {
        var ctx = new ChannelContext(this, this.Handlers);
        do
        {
            ctx.NextHandle();
            await ctx.Current.Disconnect(ctx);
        } while (ctx.HasNext());
    }
}
