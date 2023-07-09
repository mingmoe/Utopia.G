using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Utopia.Core.Utilities.IO;

namespace Utopia.Core.Net;

/// <summary>
/// 分包器,要求是线程安全的.
/// </summary>
public interface IPacketizer
{
    public void OperateFormatterList(Action<IList<IPacketFormatter>> action);

    public bool TryGetFormatter(Guuid id, out IPacketFormatter? formatter);

    /// <summary>
    /// 把二进制转换为包
    /// </summary>
    public object ConvertPacket(Guuid packetTypeId, byte[] data);

    /// <summary>
    /// 把包序列化后，写入包的长度(将调用<see cref="System.Net.IPAddress.HostToNetworkOrder(int)"/>
    /// 和包体.
    /// </summary>
    public byte[] WritePacket(Guuid packetTypeId, object obj);
}

/// <summary>
/// 分包器,是线程安全的.
/// </summary>
public class Packetizer : IPacketizer
{
    private readonly object _locker = new();

    private readonly SafeList<IPacketFormatter> _formatters = new();

    public void OperateFormatterList(Action<IList<IPacketFormatter>> action)
    {
        lock (this._locker)
        {
            this._formatters.EnterList(action);
        }
    }

    /// <summary>
    /// without locking
    /// </summary>
    /// <param name="id"></param>
    /// <param name="formatter"></param>
    /// <returns></returns>
    private bool _TryGetFormatter(Guuid id, out IPacketFormatter? formatter)
    {
        _formatters.EnterList((l) =>
        {
            tryGetFormatter(l);
        });

        IPacketFormatter? result = null;
        void tryGetFormatter(IList<IPacketFormatter> list)
        {
            foreach (var f in list)
            {
                if (f.Id.Equals(id))
                {
                    result = f;
                }
            }
        }

        formatter = result;

        return result != null;
    }

    public static async Task<(Guuid, byte[])> ReadPacket(Stream stream)
    {
        Guard.IsNotNull(stream);

        var packet = await StreamUtility.ReadPacket(stream)!;
        var packStream = new MemoryStream(packet);
        var id = await StreamUtility.ReadGuuid(packStream);
        var data = packet[((int)packStream.Position)..];

        return (id, data);
    }

    public bool TryGetFormatter(Guuid id, out IPacketFormatter? formatter)
    {
        lock (this._locker)
        {
            return this._TryGetFormatter(id, out formatter);
        }
    }

    public object ConvertPacket(Guuid packetTypeId, byte[] data)
    {
        lock (this._locker)
        {
            if (this.TryGetFormatter(packetTypeId, out IPacketFormatter? formatter))
            {
                return formatter!.GetValue(data);
            }
            throw new InvalidOperationException("unknown packet type id");
        }
    }

    /// <summary>
    /// 把包序列化后，写入包的长度(将调用<see cref="System.Net.IPAddress.HostToNetworkOrder(int)"/>
    /// 和包体.
    /// </summary>
    public byte[] WritePacket(Guuid packetTypeId, object obj)
    {
        lock (this._locker)
        {
            if (this._TryGetFormatter(packetTypeId, out IPacketFormatter? formatter))
            {
                var data = formatter!.ToPacket(obj);

                var stream = new MemoryStream(data.Length + 4);

                stream.Write(BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder(data.Length)));
                stream.Write(data);
                return stream.ToArray();
            }
            throw new InvalidOperationException("unknown packet type id");
        }
    }
}
