//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//

namespace Utopia.Core;

/// <summary>
/// 监控一个变量是否已更改
/// </summary>
public interface IEventVariable<T>
{
    T Value { get; set; }

    IEventManager<IEvent> ModifyEvent { get; }
}
