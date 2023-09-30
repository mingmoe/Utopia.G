#region copyright
// This file(may named Ticker.cs) is a part of the project: Utopia.Server.
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

using System.Diagnostics;

namespace Utopia.Server.Logic;

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

    public long MillisecondFromLastTick => this._stopwatch.ElapsedMilliseconds;

    public void Start()
    {
        this._stopwatch.Start();
    }

    public void Tick()
    {
        this.TickCount++;
        this._stopwatch.Restart();
    }
}
