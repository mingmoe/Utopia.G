//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//

namespace Utopia.Core.Net
{
    /// <summary>
    /// 网络数据处理者
    /// </summary>
    public interface IHandler
    {
        Task<object?> Write(IChannelContext ctx, object? input)
        {
            return new Task<object?>(() => { return input; });
        }

        Task<object?> Read(IChannelContext ctx, object? input)
        {
            return new Task<object?>(() => { return input; });
        }

        Task Connect(IChannelContext ctx) { return new Task(() => { }); }

        Task Disconnect(IChannelContext ctx) { return new Task(() => { }); }
    }
}
