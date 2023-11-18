// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using static Utopia.Benchmark.Program;

namespace Utopia.Benchmark;

[Config(typeof(Configuration))]
public class DictionaryBenchmark
{
    private readonly Dictionary<string, string> _common = new();

    private readonly FrozenDictionary<string, string> _frozen;

    private readonly List<string> _keys = new();

    private ushort _common_index = 0;

    private ushort _frozen_index = 0;

    public DictionaryBenchmark()
    {
        for(int index=0;index!=ushort.MaxValue;index++)
        {
            var key = Utility.RandomString(64);
            var value = Utility.RandomString(64);

            _keys.Add(key);

            _common.Add(key, value);
        }
        _frozen = _common.ToFrozenDictionary();
    }

    [Benchmark]
    public string CommonDictionary()
    {
        var _ = _common.TryGetValue(_keys[_common_index],out string? output);
        _common_index++;
        return output!;
    }

    [Benchmark]
    public string FrozenDictionary()
    {
        var _ = _frozen.TryGetValue(_keys[_frozen_index], out string? output);
        _frozen_index++;
        return output!;
    }
}
