// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Collections;

namespace Utopia.Core.Collections;

/// <summary>
/// 一个线程安全的list
/// </summary>
public interface ISafeList<T> : IRWSynchronizable,IList<T>, ISynchronizedOperation<IList<T>>, ISynchronizedOperation<IReadOnlyList<T>>
{
}

/// <summary>
/// There are some ways to access this object in a thread-safe way.
/// <br/>
/// First:
/// call list.<see cref="ISynchronizedOperation{T}.EnterSync(Action{T})"/>.
/// <br/>
/// Second:
/// Enter the <see cref="IRWSynchronizable.@lock"/> lock.
/// <br/>
/// That's all. Good luck.
/// </summary>
/// <typeparam name="T"></typeparam>
public class SafeList<T> : List<T>, ISafeList<T>
{
    protected ReaderWriterLockSlim _rwLock = new();

    public ReaderWriterLockSlim @lock => _rwLock;

    public void EnterSync(Action<IList<T>> action)
    {
        @lock.EnterWriteLock();
        try
        {
            action.Invoke(this);
        }
        finally
        {
            @lock.ExitWriteLock();
        }
    }

    public void EnterSync(Action<IReadOnlyList<T>> action)
    {
        @lock.EnterReadLock();
        try
        {
            action.Invoke(this);
        }
        finally
        {
            @lock.ExitReadLock();
        }
    }
}

