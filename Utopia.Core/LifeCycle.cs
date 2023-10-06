using NLog;
using Utopia.Core.Events;

namespace Utopia.Core;

public enum LifeCycleOrder
{
    Before,
    After,
}

public class LifeCycleEvent<CycleT>
{
    public LifeCycleOrder Order { get; init; }

    public CycleT Cycle { get; init; }

    public LifeCycleEvent(LifeCycleOrder order, CycleT cycle)
    {
        this.Cycle = cycle;
        this.Order = order;
    }

    public static void EnterCycle(CycleT cycle, Action action, ILogger logger, IEventBus bus, IServiceProvider provider)
    {
        if (!provider.HasService<CycleT>())
        {
            provider.TryRegisterService(cycle);
        }

        ArgumentNullException.ThrowIfNull(cycle);
        logger.Info("enter pre-{} lifecycle", cycle);
        bus!.Fire(new LifeCycleEvent<CycleT>(LifeCycleOrder.Before, cycle));
        logger.Info("enter {} lifecycle", cycle);
        provider.TryUpdate(provider.GetService<CycleT>(), cycle);
        action.Invoke();
        logger.Info("enter post-{} lifecycle", cycle);
        bus!.Fire(new LifeCycleEvent<CycleT>(LifeCycleOrder.After, cycle));

    }
}
