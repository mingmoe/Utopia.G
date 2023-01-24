//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utopia.Core.Net;

/// <summary>
/// 通常是Debug使用，回写所有接收到的数据
/// </summary>
public class EchoHandler : IHandler
{
    public async Task<object?> Read(IChannelContext ctx, object? input)
    {
        await ctx.Write(input);
        return input;
    }

    public async Task Connect(IChannelContext ctx) {
        var m = new Memory<byte>(Encoding.UTF8.GetBytes("connected"));
        await ctx.Write(m);
    }
}
