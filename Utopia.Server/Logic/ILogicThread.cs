// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using CommunityToolkit.Diagnostics;
using Utopia.Core.Collections;

namespace Utopia.Server.Logic;

/// <summary>
/// 逻辑线程，按照tick进行更新操作。
/// </summary>
public interface ILogicThread : ISafeList<IUpdatable>
{
    /// <summary>
    /// 逻辑线程更新器
    /// </summary>
    IUpdater Updater { get; }

    /// <summary>
    /// 已经进行了多少次Tick
    /// </summary>
    long Ticks { get; }

    CancellationTokenSource StopTokenSource { get; }
}

public class StandardLogicThread : SafeList<IUpdatable>,ILogicThread
{
    public StandardLogicThread(ITicker ticker, IUpdater updater)
    {
        _ticker = ticker;
        Updater = updater;
    }

    public StandardLogicThread() : this(new Ticker(), new SimplyUpdater()) { }

    private readonly ITicker _ticker;

    public IUpdater Updater { get; }

    public long Ticks => _ticker.MillisecondFromLastTick;

    public CancellationTokenSource StopTokenSource { get; } = new();

    public void Run(CancellationTokenSource startTokenSource)
    {
        CancellationToken token = StopTokenSource.Token;

        _ticker.Start();

        while (true)
        {
            startTokenSource.CancelAfter(100/* wait for fun :-) */);

            while (!token.IsCancellationRequested)
            {
                IUpdatable[] todos;

                _rwLock.EnterReadLock();
                try
                {
                    todos = ToArray();
                }
                finally
                {
                    _rwLock.ExitReadLock();
                }

                foreach (IUpdatable task in todos)
                {
                    task.Update(Updater);
                }
            }
            _ticker.WaitToNextTick();
            _ticker.Tick();
        }
    }
}
