using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        this._new = new Guuid(_data.First(), _data[1..]);
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
        return Guuid.ParseString(this._converted);
    }

    [Benchmark]
    public string ToStringBenchmark()
    {
        return this._new.ToString();
    }

}
