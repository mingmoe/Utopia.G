// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Autofac.Core;

namespace Utopia.Core;

/// <summary>
/// You MUST call <see cref="IDisposable.Dispose"/> to release the lock.
/// And this should be call manually and only once.
/// </summary>
/// <typeparam name="T"></typeparam>
public struct SynchronizedHanlder<T> : IDisposable
{
    private volatile int _disposed = 0;
    private readonly object _lock;

    public void Dispose()
    {
        int read = Interlocked.Exchange(ref _disposed, 1);
        if(read == 1) return;
        Monitor.Exit(_lock);
        GC.SuppressFinalize(this);
    }

    public SynchronizedHanlder(ISynchronizable obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        _lock = obj.@lock;
        Value = (T)obj;
    }

    public SynchronizedHanlder(T obj,object @lock)
    {
        ArgumentNullException.ThrowIfNull(obj);
        ArgumentNullException.ThrowIfNull(@lock);
        _lock = @lock;
        Value = obj;
    }

    public readonly T Value;
}

public readonly struct SynchronizedGuard<T> : ISynchronizedOperation<T> where T : ISynchronizable
{
    private readonly T _object;

    public SynchronizedGuard(T obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        _object = obj;
    }

    public SynchronizedHanlder<T> EnterLock()
    {
        Monitor.Enter(_object.@lock);
        return new SynchronizedHanlder<T>(_object, _object.@lock);
    }

    public void EnterSync(Action<T> action) => EnterThenExit(action);

    public void EnterThenExit(Action<T> action)
    {
        using var @lock = EnterLock();

        action.Invoke(@lock.Value);
    }
}

/// <summary>
/// You MUST call <see cref="IDisposable.Dispose"/> to release the lock.
/// And this should be call manually and only once.
/// </summary>
public struct ReaderWriterSynchronizedHanlder<T> : IDisposable
{
    private volatile int _disposed = 0;
    private readonly ReaderWriterLockSlim _lock;
    private readonly bool _exitWrite;

    public void Dispose()
    {
        int read = Interlocked.Exchange(ref _disposed, 1);
        if (read == 1) return;

        if (_exitWrite)
            _lock.ExitWriteLock();
        else
            _lock.ExitReadLock();
        GC.SuppressFinalize(this);
    }

    public ReaderWriterSynchronizedHanlder(T obj,ReaderWriterLockSlim @lock,bool exitWrite)
    {
        ArgumentNullException.ThrowIfNull(obj);
        ArgumentNullException.ThrowIfNull(@lock);
        _lock = @lock;
        Value = obj;
        _exitWrite = exitWrite;
    }

    public readonly T Value;
}

public readonly struct ReaderWriterSynchronizedGuard<T> : ISynchronizedOperation<T> where T : IRWSynchronizable
{
    private readonly T _object;

    public ReaderWriterSynchronizedGuard(T obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        _object = obj;
    }

    public ReaderWriterSynchronizedHanlder<T> EnterWriteLock()
    {
        _object.@lock.EnterWriteLock();
        return new ReaderWriterSynchronizedHanlder<T>(_object, _object.@lock,true);
    }

    public ReaderWriterSynchronizedHanlder<T> EnterReadLock()
    {
        _object.@lock.EnterReadLock();
        return new ReaderWriterSynchronizedHanlder<T>(_object,_object.@lock,false);
    }

    public void EnterWriteThenExit(Action<T> action)
    {
        using var @lock = EnterWriteLock();

        action.Invoke(@lock.Value);
    }

    public void EnterReadThenExit(Action<T> action)
    {
        using var @lock = EnterReadLock();

        action.Invoke(@lock.Value);
    }

    public void EnterSync(Action<T> action) => EnterWriteThenExit(action);
}

public static class SynchronizedUtils
{
    public static ReaderWriterSynchronizedHanlder<T> EnterWriteLock<T>(this T obj) where T : IRWSynchronizable
    {
        obj.@lock.EnterWriteLock();
        return new ReaderWriterSynchronizedHanlder<T>(obj, obj.@lock, true);
    }

    public static ReaderWriterSynchronizedHanlder<T> EnterReadLock<T>(this T obj) where T : IRWSynchronizable
    {
        obj.@lock.EnterReadLock();
        return new ReaderWriterSynchronizedHanlder<T>(obj, obj.@lock, false);
    }

    public static SynchronizedHanlder<T> EnterLock<T>(this T obj) where T : ISynchronizable
    {
        Monitor.Enter(obj);
        return new SynchronizedHanlder<T>(obj,obj.@lock);
    }
}
