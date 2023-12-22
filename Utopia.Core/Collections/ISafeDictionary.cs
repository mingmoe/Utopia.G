// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Collections.Generic;

namespace Utopia.Core.Collections;

/// <summary>
/// 一个比<see cref="System.Collections.Concurrent.ConcurrentDictionary{TKey, TValue}"/>更加安全的Dictionary，保证任何调用都处在线程安全的范围内。
/// 文档见<see cref="System.Collections.Concurrent.ConcurrentDictionary{TKey, TValue}"/>
/// </summary>
public interface ISafeDictionary<KeyT, ValueT> : IReadOnlyDictionary<KeyT,ValueT> where KeyT : notnull
{
    /// <summary>
    /// 清除所有字典
    /// </summary>
    void Clear();

    KeyValuePair<KeyT, ValueT>[] ToArray();

    /// <summary>
    /// 如果键尚不存在，则使用指定函数和参数将键/值对添加到Dictionary，如果键存在，则返回现有值。
    /// </summary>
    ValueT GetOrAdd<ArgT>(KeyT key, Func<KeyT, ArgT, ValueT> valueFactory, ArgT factoryArgument);

    /// <summary>
    /// 如果键尚不存在，则使用指定函数和参数将键/值对添加到Dictionary，如果键存在，则返回现有值。
    /// </summary>
    ValueT GetOrAdd(KeyT key, Func<KeyT, ValueT> valueFactory);

    bool TryAdd(KeyT key, ValueT value);

    bool TryRemove(KeyT key, out ValueT? value);

    bool TryUpdate(KeyT key, ValueT newValue, ValueT comparisonValue);
}
