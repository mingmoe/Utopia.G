using BenchmarkDotNet.Running;

namespace Utopia.Benchmark;

internal class Program
{
    static void Main(string[] _)
    {
        BenchmarkRunner.Run<GuuidBenchmark>();
    }
}
