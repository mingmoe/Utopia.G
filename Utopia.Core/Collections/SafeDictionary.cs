// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Collections.Concurrent;

namespace Utopia.Core.Collections;

public class SafeDictionary<KeyT, ValueT> : ISafeDictionary<KeyT, ValueT> where KeyT : notnull
{
    protected readonly ConcurrentDictionary<KeyT, ValueT> _safeDictionary = new();

    public void Clear() => _safeDictionary.Clear();

    public bool ContainsKey(KeyT key) => _safeDictionary.ContainsKey(key);

    public ValueT GetOrAdd<ArgT>(KeyT key, Func<KeyT, ArgT, ValueT> valueFactory, ArgT factoryArgument) => _safeDictionary.GetOrAdd(key, valueFactory, factoryArgument);

    public ValueT GetOrAdd(KeyT key, Func<KeyT, ValueT> valueFactory) => _safeDictionary.GetOrAdd(key, valueFactory);

    public KeyValuePair<KeyT, ValueT>[] ToArray() =>
        // this should be thread safe
        _safeDictionary.ToArray();

    public bool TryAdd(KeyT key, ValueT value) => _safeDictionary.TryAdd(key, value);

    public bool TryGetValue(KeyT key, out ValueT? value) => _safeDictionary.TryGetValue(key, out value);

    public bool TryRemove(KeyT key, out ValueT? value) => _safeDictionary.TryRemove(key, out value);

    public bool TryUpdate(KeyT key, ValueT newValue, ValueT comparisonValue) => _safeDictionary.TryUpdate(key, newValue, comparisonValue);

    public ValueT AddOrUpdate(KeyT key, Func<KeyT, ValueT> newFactory, Func<KeyT, ValueT, ValueT> updateFactory) => _safeDictionary.AddOrUpdate(key, newFactory, updateFactory);
    public void EnterSync(Action<IDictionary<KeyT, ValueT>> action) => throw new NotImplementedException();
}
