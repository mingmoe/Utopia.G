// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using BenchmarkDotNet.Running;

namespace Utopia.Benchmark;

internal class Program
{
    private static void Main(string[] _) => BenchmarkRunner.Run<GuuidBenchmark>();
}
