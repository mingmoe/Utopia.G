using CommunityToolkit.Diagnostics;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utopia.Core.Map;
using Utopia.Core.Utilities;

namespace Utopia.Core.Net.Packet;

/// <summary>
/// 查询地图包
/// </summary>
[MessagePackObject]
public class QueryMapPacket
{
    [Key(0)]
    public WorldPosition QueryPosition { get; set; }

    [Key(1)]
    public Guuid[]? Entities { get; set; }

    [Key(2)]
    public bool? Accessible { get; set; }

    [Key(3)]
    public bool? Collidable { get; set; }
}

public class QueryMapPacketFormatter : IPacketFormatter
{
    public static readonly Guuid PacketTypeId = new("utopia", "core", "net", "packet", "query", "block");

    public Guuid Id => PacketTypeId;

    public object GetValue(byte[] packet)
    {
        return MessagePackSerializer.Deserialize<QueryMapPacket>(packet);
    }

    public byte[] ToPacket(object value)
    {
        Guard.IsNotNull(value);
        Guard.IsAssignableToType(value, typeof(QueryMapPacket));
        return MessagePackSerializer.Serialize((QueryMapPacket)value);
    }
}
