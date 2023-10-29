// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utopia.Core;

/// <summary>
/// A object that have a lock object.
/// </summary>
public interface ISynchronizable
{
    object @lock {  get; }
}

/// <summary>
/// A object that have a Write-Read lock object.
/// </summary>
public interface IRWSynchronizable{
    ReaderWriterLockSlim @lock { get; }
}

/// <summary>
/// A object that can invoke an action,with the relevant resource have been locked.
/// </summary>
/// <typeparam name="T">the type of the object you want. Its relevant resource have been locked.</typeparam>
public interface ISynchronizedOperation<T>
{
    void EnterSync(Action<T> action);
}
