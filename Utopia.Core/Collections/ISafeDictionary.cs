#region copyright
// This file(may named ISafeDictionary.cs) is a part of the project: Utopia.Core.
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

namespace Utopia.Core.Collections;

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
