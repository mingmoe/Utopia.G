// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

namespace Utopia.Core.Events;

/// <summary>
/// 事件管理器，应该是线程安全的。
/// </summary>
public interface IEventManager<EventT>
{
    void Register(Action<EventT> handler);

    void Unregister(Action<EventT> handler);

    void ClearRegisters();

    void Fire(EventT e);
}
