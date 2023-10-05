#region copyright
// This file(may named ILogicThread.cs) is a part of the project: Utopia.Server.
// 
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// 
// This file is part of Utopia.Server.
//
// Utopia.Server is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Utopia.Server is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with Utopia.Server. If not, see <https://www.gnu.org/licenses/>.
#endregion

using CommunityToolkit.Diagnostics;
using Utopia.Core.Collections;

namespace Utopia.Server.Logic;

/// <summary>
/// 逻辑线程，按照tick进行更新操作。
/// </summary>
public interface ILogicThread
{

    /// <summary>
    /// 逻辑线程更新器
    /// </summary>
    IUpdater Updater { get; }

    /// <summary>
    /// 已经进行了多少次Tick
    /// </summary>
    long Ticks { get; }

    /// <summary>
    /// 运行逻辑线程，应该在单独线程上调用，将堵塞
    /// </summary>
    void Run();

    /// <summary>
    /// 停止逻辑线程，并且不应该再次启动。即逻辑线程是一次性的
    /// </summary>
    void Stop();

    /// <summary>
    /// 添加要进行更新的操作
    /// </summary>
    void AddUpdatable(IUpdatable updatable);

    /// <summary>
    /// Remove the updatable object which has been added.
    /// </summary>
    /// <param name="updatable"></param>
    void RemoveUpdatable(IUpdatable updatable);
}

public class SimplyLogicThread : ILogicThread
{
    public SimplyLogicThread(ITicker ticker, IUpdater updater)
    {
        this._ticker = ticker;
        this.Updater = updater;
    }

    public SimplyLogicThread() : this(new Ticker(), new SimplyUpdater()) { }

    private readonly ITicker _ticker;
    private readonly SafeList<IUpdatable> _updatables = new();
    private readonly object _lock = new();
    volatile bool _isRunning = true;
    volatile bool _started = false;

    public IUpdater Updater { get; }

    public long Ticks => this._ticker.MillisecondFromLastTick;

    public void Run()
    {
        lock (this._lock)
        {
            if (this._started)
            {
                throw new InvalidOperationException("The thread has started");
            }
            this._started = true;
        }

        this._ticker.Start();

        while (true)
        {
            while (this._isRunning)
            {
                IUpdatable[] todos;
                lock (this._lock)
                {
                    todos = this._updatables.ToArray();
                }

                foreach (var task in todos)
                {
                    task.Update(this.Updater);
                }
            }
            this._ticker.WaitToNextTick();
            this._ticker.Tick();
        }
    }

    public void Stop()
    {
        this._isRunning = false;
    }

    public void AddUpdatable(IUpdatable updatable)
    {
        Guard.IsNotNull(updatable);

        lock (this._lock)
        {
            this._updatables.Add(updatable);
        }
    }

    public void RemoveUpdatable(IUpdatable updatable)
    {
        Guard.IsNotNull(updatable);

        lock (this._lock)
        {
            this._updatables.Remove(updatable);
        }
    }
}
