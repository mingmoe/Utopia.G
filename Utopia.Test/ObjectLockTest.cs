// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utopia.Core;

namespace Utopia.Test;

internal class ObjectLock : ISynchronizable
{
    public object @lock { get; init; } = new();
}

public class ObjectLockTest
{
    private ObjectLock _lock = new();

    private void TryEnterButShouldFail()
    {
        // current thread hasn't hold the lock
        Assert.False(Monitor.TryEnter(_lock.@lock));
        Assert.False(Monitor.IsEntered(_lock.@lock));
    }

    [Fact]
    public void TestLock()
    {
        Thread t = new(TryEnterButShouldFail);
        using (var _ = _lock.EnterLock())
        {
            Assert.True(Monitor.IsEntered(_lock.@lock));
            t.Start();
            t.Join();
        }
        Assert.False(Monitor.IsEntered(_lock.@lock));
    }
}

internal class RwLock : IRWSynchronizable
{
    public ReaderWriterLockSlim @lock { get; init; } = new();
}

public class RwLockTest
{

    private RwLock _lock = new();

    private void TryEnterReadAndSuccess()
    {
        Assert.True(_lock.@lock.TryEnterReadLock(1));
        _lock.@lock.ExitReadLock();
    }

    private void TryEnterReadButFail()
    {
        Assert.False(_lock.@lock.TryEnterReadLock(1));
    }

    [Fact]
    public void TestReadLock()
    {
        using (var _ = _lock.EnterReadLock())
        {
            Thread t = new(TryEnterReadAndSuccess);
            t.Start();
            Thread t2 = new(TryEnterReadAndSuccess);
            t2.Start();
            t.Join();
            t2.Join();
        }
        Assert.Equal(0, _lock.@lock.CurrentReadCount);
    }

    [Fact]
    public void TestWriteLock()
    {
        using (var _ = _lock.EnterWriteLock())
        {
            Thread t = new(TryEnterReadButFail);
            t.Start();
            Thread t2 = new(TryEnterReadButFail);
            t2.Start();
            t.Join();
            t2.Join();
        }
        Assert.Equal(0, _lock.@lock.CurrentReadCount);
    }
}
