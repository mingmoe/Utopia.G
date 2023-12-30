// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Utopia.Core.Net;

/// <summary>
/// Stand for a socket
/// </summary>
public interface ISocket : IDisposable
{
    Task Write(ReadOnlyMemory<byte> data);

    Task<int> Read(Memory<byte> dst);

    void Shutdown();

    bool Alive { get; }

    /// <summary>
    /// If null,the socket may not based on real socket and has no real remote address
    /// </summary>
    EndPoint? RemoteAddress { get; }
}
