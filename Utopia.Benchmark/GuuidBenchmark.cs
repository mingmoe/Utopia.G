#region copyright
// This file(may named GuuidBenchmark.cs) is a part of the project: Utopia.Benchmark.
// 
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// 
// This file is part of Utopia.Benchmark.
//
// Utopia.Benchmark is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Utopia.Benchmark is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with Utopia.Benchmark. If not, see <https://www.gnu.org/licenses/>.
#endregion

using BenchmarkDotNet.Attributes;
using Utopia.Core.Utilities;

namespace Utopia.Benchmark;

/// <summary>
/// 对于Guuid的基准测试
/// </summary>
public class GuuidBenchmark
{
    private readonly string[] _data = Utility.RandomStringArray(32);

    private readonly string _converted;

    private readonly Guuid _new;

    public GuuidBenchmark()
    {
        this._new = new Guuid(this._data.First(), this._data[1..]);
        this._converted = this._new.ToString();
    }

    [Benchmark]
    public Guuid Construction()
    {
        return new Guuid(this._data.First(), this._data[1..]);
    }

    [Benchmark]
    public Guuid PauseGuuid()
    {
        return Guuid.Parse(this._converted);
    }

    [Benchmark]
    public string ToStringBenchmark()
    {
        return this._new.ToString();
    }

}
