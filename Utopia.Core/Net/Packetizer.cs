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
/// 分包器
/// </summary>
public class Packetizer
{
    public readonly List<IPacketFormatter> Formatters = new();

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
        Guard.IsNotNull(id);

        foreach (var f in this.Formatters)
        {
            if (f.Id.Equals(id))
            {
                formatter = f;
                return true;
            }
        }
        formatter = null;
        return false;
    }

    public object ConvertPacket(Guuid packetTypeId, byte[] data)
    {
        if (this.TryGetFormatter(packetTypeId, out IPacketFormatter? formatter))
        {
            return formatter!.GetValue(data);
        }
        throw new InvalidOperationException("unknown packet type id");
    }

    /// <summary>
    /// 把包序列化后，写入包的长度(将调用<see cref="System.Net.IPAddress.HostToNetworkOrder(int)"/>
    /// </summary>
    public byte[] WritePacket(Guuid packetTypeId, object obj)
    {
        if (this.TryGetFormatter(packetTypeId, out IPacketFormatter? formatter))
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
