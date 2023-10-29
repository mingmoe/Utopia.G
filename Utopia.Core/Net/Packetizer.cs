// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using Utopia.Core.Collections;
using Utopia.Core.Utilities;

namespace Utopia.Core.Net;

/// <summary>
/// 分包器,要求是线程安全的.
/// </summary>
public interface IPacketizer : ISynchronizedOperation<IList<IPacketFormatter>>
{
    public bool TryGetFormatter(Guuid id, out IPacketFormatter? formatter);

    /// <summary>
    /// 把字节序列转换为包.
    /// </summary>
    public object ConvertPacket(Guuid packetTypeId, byte[] data);

    /// <summary>
    /// 把包转化为字节序列
    /// </summary>
    public byte[] WritePacket(Guuid packetTypeId, object obj);
}

/// <summary>
/// 分包器,是线程安全的.
/// </summary>
public class Packetizer : SafeList<IPacketFormatter>,IPacketizer
{
    /// <summary>
    /// without locking
    /// </summary>
    private bool _TryGetFormatter(Guuid id, out IPacketFormatter? formatter)
    {
        IPacketFormatter? result = null;
        foreach (IPacketFormatter f in this)
        {
            if (f.Id.Equals(id))
            {
                result = f;
            }
        }
    

        formatter = result;

        return result != null;
    }

    public bool TryGetFormatter(Guuid id, out IPacketFormatter? formatter)
    {
        @lock.EnterReadLock();
        try
        {
            return _TryGetFormatter(id, out formatter);
        }
        finally
        {
            @lock.ExitReadLock();
        }
    }

    public object ConvertPacket(Guuid packetTypeId, byte[] data)
    {
        return TryGetFormatter(packetTypeId, out IPacketFormatter? formatter)
            ? formatter!.GetValue(data)
            : throw new InvalidOperationException("unknown packet type id");
    }

    public byte[] WritePacket(Guuid packetTypeId, object obj)
    {
        if (TryGetFormatter(packetTypeId, out IPacketFormatter? formatter))
        {
            byte[] data = formatter!.ToPacket(obj);

            return data;
        }
        throw new InvalidOperationException("unknown packet type id");
    }
}
