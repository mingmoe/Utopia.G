#region copyright
// This file(may named LifeCycle.cs) is a part of the project: Utopia.G.
// 
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// 
// This file is part of Utopia.G.
//
// Utopia.G is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Utopia.G is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with Utopia.G. If not, see <https://www.gnu.org/licenses/>.
#endregion

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
