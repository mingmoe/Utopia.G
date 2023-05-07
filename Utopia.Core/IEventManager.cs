//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//

namespace Utopia.Core;

/// <summary>
/// 事件管理器
/// </summary>
public interface IEventManager<EventT>
{
    delegate void Handler(EventT e);

    void Register(Handler handler);

    void Unregister(Handler handler);

    void ClearRegister();

    void Fire(EventT e);
}
