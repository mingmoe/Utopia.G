using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utopia.Server;

/// <summary>
/// 游戏时间管理的核心，负责高精度计时，非线程安全
/// </summary>
public interface ITicker
{
    const int TicksPerSecond = 20;

    const int MillisecondPerTick = 1000 / TicksPerSecond;

    /// <summary>
    /// Start to tick. Never stop
    /// </summary>
    void Start();

    /// <summary>
    /// 计时
    /// </summary>
    void Tick();

    /// <summary>
    /// 从开始计时已经tick多少次了
    /// </summary>
    ulong TickCount { get; }

    long MillisecondFromLastTick { get; }

    void WaitToNextTick()
    {
        while (this.MillisecondFromLastTick < MillisecondPerTick)
        {
            Thread.Yield();
        }
    }
}

internal class Ticker : ITicker
{
    private readonly Stopwatch _stopwatch = new();

    public ulong TickCount { get; private set; }

    public long MillisecondFromLastTick => _stopwatch.ElapsedMilliseconds;

    public void Start()
    {
        _stopwatch.Start();
    }

    public void Tick()
    {
        this.TickCount++;
        _stopwatch.Restart();
    }
}
