using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utopia.Core;

/// <summary>
/// 一个比<see cref="System.Collections.Concurrent.ConcurrentDictionary{TKey, TValue}"/>更加安全的Dictionary，保证任何调用都处在线程安全的范围内。
/// 文档见<see cref="System.Collections.Concurrent.ConcurrentDictionary{TKey, TValue}"/>
/// </summary>
public interface ISafeDictionary<KeyT, ValueT> where KeyT : notnull
{
    /// <summary>
    /// 清除所有字典
    /// </summary>
    void Clear();

    /// <summary>
    /// 检查是否包含某个键
    /// </summary>
    /// <param name="key">要检查的键，必须为非null</param>
    /// <returns>如果键存在，则返回true</returns>
    bool ContainsKey(KeyT key);

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

    bool TryGetValue(KeyT key, out ValueT? value);

    bool TryRemove(KeyT key, out ValueT? value);

    bool TryUpdate(KeyT key, ValueT newValue, ValueT comparisonValue);
}