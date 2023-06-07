using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}

public class SimplyLogicThread : ILogicThread
{
    public SimplyLogicThread(ITicker ticker, IUpdater updater)
    {
        this._ticker = ticker;
        this._updater = updater;
    }

    public SimplyLogicThread() : this(new Ticker(), new SimplyUpdater()) { }

    private readonly ITicker _ticker;
    private readonly IUpdater _updater;
    private readonly List<IUpdatable> _updatables = new();
    private readonly object _lock = new();
    volatile bool _isRunning = true;
    volatile bool _started = false;

    public IUpdater Updater => _updater;

    public long Ticks => _ticker.MillisecondFromLastTick;

    public void Run()
    {
        lock (this._lock)
        {
            if (this._started)
            {
                throw new InvalidOperationException("The thread has started");
            }
            _started = true;
        }

        this._ticker.Start();

        while (true)
        {
            while (_isRunning)
            {
                IUpdatable[] todos;
                lock (this._lock)
                {
                    todos = this._updatables.ToArray();
                }

                foreach (var task in todos)
                {
                    task.Update(this._updater);
                }
            }
            this._ticker.WaitToNextTick();
            this._ticker.Tick();
        }
    }

    public void Stop()
    {
        _isRunning = false;
    }

    public void AddUpdatable(IUpdatable updatable)
    {
        Guard.IsNotNull(updatable);

        lock (this._lock)
        {
            this._updatables.Add(updatable);
        }
    }
}
