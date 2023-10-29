// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

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
        _new = new Guuid(_data.First(), _data[1..]);
        _converted = _new.ToString();
    }

    [Benchmark]
    public Guuid ConstructionGuuid() => new(_data.First(), _data[1..]);

    [Benchmark]
    public string ConstructionString() => new(_converted);
    [Benchmark]
    public Guuid PauseGuuid() => Guuid.Parse(_converted);

    [Benchmark]
    public string StringToString() => _converted.ToString();

    [Benchmark]
    public string GuuidToString() => _new.ToString();

    [Benchmark]
    public int GuuidHashCode() => _new.GetHashCode();

    [Benchmark]
    public int StringHashCode() => _converted.GetHashCode();
}
