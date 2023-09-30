#region copyright
// This file(may named Program.cs) is a part of the project: Utopia.Benchmark.
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

using BenchmarkDotNet.Running;

namespace Utopia.Benchmark;

internal class Program
{
    static void Main(string[] _)
    {
        BenchmarkRunner.Run<GuuidBenchmark>();
    }
}
