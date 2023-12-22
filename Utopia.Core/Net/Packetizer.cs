// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Buffers;
using Utopia.Core.Collections;
using Utopia.Core.Utilities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Utopia.Core.Net;

/// <summary>
/// 分包器,要求是线程安全的.
/// </summary>
public interface IPacketizer : ISafeDictionary<Guuid,IPacketFormatter>
{
    /// <summary>
    /// 把字节序列转换为包.
    /// </summary>
    public object ConvertPacket(Guuid packetTypeId, ReadOnlySequence<byte> data);

    /// <summary>
    /// 把包转化为字节序列
    /// </summary>
    public Memory<byte> WritePacket(Guuid packetTypeId, object obj);
}

/// <summary>
/// 分包器,是线程安全的.
/// </summary>
public class Packetizer : SafeDictionary<Guuid,IPacketFormatter>, IPacketizer
{

    public object ConvertPacket(Guuid packetTypeId, ReadOnlySequence<byte> data)
    {
        if(!TryGetValue(packetTypeId,out var formatter))
        {
            throw new InvalidOperationException("unknown packet type id");
        }

        return formatter.GetValue(packetTypeId,data);
    }

    public Memory<byte> WritePacket(Guuid packetTypeId, object obj)
    {
        if (!TryGetValue(packetTypeId, out var formatter))
        {
            throw new InvalidOperationException("unknown packet type id");
        }

        return formatter.ToPacket(packetTypeId,obj);
    }
}
