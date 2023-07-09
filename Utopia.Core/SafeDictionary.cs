using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utopia.Core;

public class SafeDictionary<KeyT, ValueT> : ISafeDictionary<KeyT, ValueT> where KeyT : notnull
{
    protected readonly ConcurrentDictionary<KeyT, ValueT> _safeDictionary = new();

    public void Clear()
    {
        _safeDictionary.Clear();
    }

    public bool ContainsKey(KeyT key)
    {
        return _safeDictionary.ContainsKey(key);
    }

    public ValueT GetOrAdd<ArgT>(KeyT key, Func<KeyT, ArgT, ValueT> valueFactory, ArgT factoryArgument)
    {
        return _safeDictionary.GetOrAdd(key, valueFactory, factoryArgument);
    }

    public ValueT GetOrAdd(KeyT key, Func<KeyT, ValueT> valueFactory)
    {
        return _safeDictionary.GetOrAdd(key, valueFactory);
    }

    public KeyValuePair<KeyT, ValueT>[] ToArray()
    {
        // this should be thread safe
        return _safeDictionary.ToArray();
    }

    public bool TryAdd(KeyT key, ValueT value)
    {
        return _safeDictionary.TryAdd(key, value);
    }

    public bool TryGetValue(KeyT key, out ValueT? value)
    {
        return _safeDictionary.TryGetValue(key, out value);
    }

    public bool TryRemove(KeyT key, out ValueT? value)
    {
        return _safeDictionary.TryRemove(key, out value);
    }

    public bool TryUpdate(KeyT key, ValueT newValue, ValueT comparisonValue)
    {
        return _safeDictionary.TryUpdate(key, newValue, comparisonValue);
    }

    public ValueT AddOrUpdate(KeyT key, Func<KeyT, ValueT> newFactory, Func<KeyT, ValueT, ValueT> updateFactory)
    {
        return _safeDictionary.AddOrUpdate(key, newFactory, updateFactory);
    }
}
