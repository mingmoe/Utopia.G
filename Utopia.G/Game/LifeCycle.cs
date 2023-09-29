using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utopia.G.Game;

public enum LifeCycle
{
    /// <summary>
    /// 注意：这个代表系统已经初始化完毕之后，并非代表系统正在初始化。
    /// 初始化的时候事件总线尚未存在。
    /// </summary>
    InitializedSystem,
    LoadPlugin,
    ConnectToServer,
    Crash,
    GracefulShutdown,
    /// <summary>
    /// 对于崩溃和正常关闭，都会存在此生命周期。
    /// </summary>
    Stop,
}

public enum LifeCycleOrder
{
    Before,
    After,
}

public class LifeCycleEvent
{
    public LifeCycleOrder Order { get; init; }

    public LifeCycle Cycle { get; init; }

    public LifeCycleEvent(LifeCycleOrder order, LifeCycle cycle)
    {
        this.Cycle = cycle;
        this.Order = order;
    }
}
