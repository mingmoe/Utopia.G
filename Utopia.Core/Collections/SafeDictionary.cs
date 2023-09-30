#region copyright
// This file(may named SafeDictionary.cs) is a part of the project: Utopia.Core.
// 
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// 
// This file is part of Utopia.Core.
//
// Utopia.Core is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Utopia.Core is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with Utopia.Core. If not, see <https://www.gnu.org/licenses/>.
#endregion

using System.Collections.Concurrent;

namespace Utopia.Core.Collections;

public class SafeDictionary<KeyT, ValueT> : ISafeDictionary<KeyT, ValueT> where KeyT : notnull
{
    protected readonly ConcurrentDictionary<KeyT, ValueT> _safeDictionary = new();

    public void Clear()
    {
        this._safeDictionary.Clear();
    }

    public bool ContainsKey(KeyT key)
    {
        return this._safeDictionary.ContainsKey(key);
    }

    public ValueT GetOrAdd<ArgT>(KeyT key, Func<KeyT, ArgT, ValueT> valueFactory, ArgT factoryArgument)
    {
        return this._safeDictionary.GetOrAdd(key, valueFactory, factoryArgument);
    }

    public ValueT GetOrAdd(KeyT key, Func<KeyT, ValueT> valueFactory)
    {
        return this._safeDictionary.GetOrAdd(key, valueFactory);
    }

    public KeyValuePair<KeyT, ValueT>[] ToArray()
    {
        // this should be thread safe
        return this._safeDictionary.ToArray();
    }

    public bool TryAdd(KeyT key, ValueT value)
    {
        return this._safeDictionary.TryAdd(key, value);
    }

    public bool TryGetValue(KeyT key, out ValueT? value)
    {
        return this._safeDictionary.TryGetValue(key, out value);
    }

    public bool TryRemove(KeyT key, out ValueT? value)
    {
        return this._safeDictionary.TryRemove(key, out value);
    }

    public bool TryUpdate(KeyT key, ValueT newValue, ValueT comparisonValue)
    {
        return this._safeDictionary.TryUpdate(key, newValue, comparisonValue);
    }

    public ValueT AddOrUpdate(KeyT key, Func<KeyT, ValueT> newFactory, Func<KeyT, ValueT, ValueT> updateFactory)
    {
        return this._safeDictionary.AddOrUpdate(key, newFactory, updateFactory);
    }
}
