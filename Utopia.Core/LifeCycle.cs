// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using NLog;
using Utopia.Core.Events;

namespace Utopia.Core;

/// <summary>
/// e.g.:
/// Before Lifecycle(Fire events) -> Enter Lifecycle(Execute the code about the lifecycle) -> After Lifecycle(Fire events)
/// </summary>
public enum LifeCycleOrder
{
    /// <summary>
    /// Before enter next lifecycle. The <see cref="LifeCycleEvent{CycleT}.Cycle"/> hasn't changed.
    /// </summary>
    Before,
    /// <summary>
    /// After enter next lifecycle.
    /// </summary>
    After,
}

public class LifeCycleEvent<CycleT> : Event
{
    public LifeCycleOrder Order { get; init; }

    public CycleT Cycle { get; init; }

    public LifeCycleEvent(LifeCycleOrder order, CycleT cycle)
    {
        Cycle = cycle;
        Order = order;
    }

    /// <summary>
    /// Abhout how we will fire the event,see <see cref="LifeCycleOrder"/>
    /// </summary>
    public static void EnterCycle(CycleT cycle, Action action, ILogger logger, IEventBus bus, IServiceProvider provider)
    {
        if (!provider.HasService<CycleT>())
        {
            _ = provider.TryRegisterService(cycle);
        }

        ArgumentNullException.ThrowIfNull(cycle);
        logger.Info("enter pre-{lifecycle} lifecycle", cycle);
        bus!.Fire(new LifeCycleEvent<CycleT>(LifeCycleOrder.Before, cycle));
        logger.Info("enter {lifecycle} lifecycle", cycle);
        _ = provider.TryUpdate(provider.GetService<CycleT>(), cycle);
        action.Invoke();
        logger.Info("enter post-{lifecycle} lifecycle", cycle);
        bus!.Fire(new LifeCycleEvent<CycleT>(LifeCycleOrder.After, cycle));
    }

    /// <summary>
    /// Abhout how we will fire the event,see <see cref="LifeCycleOrder"/>
    /// </summary>
    public static void EnterCycle(CycleT cycle, Action action, ILogger logger, IEventManager<LifeCycleEvent<CycleT>> bus, Action switchAction)
    {
        ArgumentNullException.ThrowIfNull(cycle);
        logger.Info("enter pre-{lifecycle} lifecycle", cycle);
        bus!.Fire(new LifeCycleEvent<CycleT>(LifeCycleOrder.Before, cycle));
        logger.Info("enter {lifecycle} lifecycle", cycle);
        switchAction.Invoke();
        action.Invoke();
        logger.Info("enter post-{lifecycle} lifecycle", cycle);
        bus!.Fire(new LifeCycleEvent<CycleT>(LifeCycleOrder.After, cycle));
    }
}
