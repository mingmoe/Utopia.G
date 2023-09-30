#region copyright
// This file(may named SafeList.cs) is a part of the project: Utopia.Core.
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
/// 一个线程安全的list
/// </summary>
public interface ISafeList<T>
{

    int Count { get; }

    public T Get(int index);

    public void Remove(T item);

    public void Insert(int index, T item);

    public void RemoveAt(int index);

    public bool Contains(T item);

    public void Add(T item);

    /// <summary>
    /// invoke the action with thread safe list
    /// </summary>
    public void EnterList(Action<IList<T>> action);

    public T[] ToArray();

    public void Clear();
}

public class SafeList<T> : ISafeList<T>
{
    protected readonly List<T> _list = new();

    protected readonly SpinLock _lock = new();

    protected void _Invoke(Action<IList<T>> action)
    {
        bool loacked = false;
        try
        {
            this._lock.Enter(ref loacked);
            action.Invoke(this._list);
        }
        finally
        {
            this._lock.Exit();
        }
    }

    public int Count
    {
        get
        {
            int result = 0;
            this._Invoke((l) => { result = l.Count; });
            return result;
        }
    }

    public void Add(T item)
    {
        this._Invoke((l) => { l.Add(item); });
    }

    public bool Contains(T item)
    {
        bool result = false;
        this._Invoke((l) => { result = l.Contains(item); });
        return result;
    }

    public T Get(int index)
    {
        T? result = default;
        this._Invoke((l) => { result = l[index]; });
        return result ?? throw new InvalidDataException("this should be not null");
    }

    public void Insert(int index, T item)
    {
        this._Invoke((l) => { l.Insert(index, item); });
    }

    public void Remove(T item)
    {
        this._Invoke((l) => { l.Remove(item); });
    }

    public void RemoveAt(int index)
    {
        this._Invoke((l) => { l.RemoveAt(index); });
    }
    public void EnterList(Action<IList<T>> action)
    {
        this._Invoke(action);
    }

    public T[] ToArray()
    {
        T[] result = Array.Empty<T>();
        this._Invoke((l) => { result = l.ToArray(); });
        return result;
    }

    public void Clear()
    {
        this._Invoke((l) => { l.Clear(); });
    }
}

