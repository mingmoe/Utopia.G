//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//

namespace Utopia.Core;

/// <summary>
/// 事件管理器，应该是线程安全的。
/// </summary>
public interface IEventManager<EventT>
{
    void Register(Action<EventT> handler);

    void Unregister(Action<EventT> handler);

    void ClearRegister();

    void Fire(EventT e);
}
