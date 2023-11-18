// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using Perfolizer.Horology;

namespace Utopia.Benchmark;

public class Program
{
    public class Configuration : ManualConfig, IConfig
    {
        public Configuration()
        {
            AddJob(
                    Job.Default
                    .WithArguments(new Argument[] { new MsBuildArgument("/p:DisableAutoVersionFileRead=true") }));
            AddDiagnoser(MemoryDiagnoser.Default);
            AddDiagnoser(ThreadingDiagnoser.Default);
            AddDiagnoser(ExceptionDiagnoser.Default);
        }
    }

    private static void Main(string[] _)
    {
        BenchmarkRunner
            .Run<GuuidBenchmark>(config: DefaultConfig.Instance);
        BenchmarkRunner.Run<DictionaryBenchmark>(config: DefaultConfig.Instance);
    }
}
