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
public class BlockInfoPacket
{

    [Key(0)]
    public Guuid[] Entities { get; set; } = Array.Empty<Guuid>();

    [Key(1)]
    public byte[][] EntityData { get; set; } = Array.Empty<byte[]>();

    [Key(2)]
    public bool? Accessible { get; set; }

    [Key(3)]
    public bool? Collidable { get; set; }

    [Key(4)]
    public WorldPosition Position { get; set; }
}

public class BlockInfoPacketFormatter : IPacketFormatter
{
    public static readonly Guuid PacketTypeId = Guuid.NewUtopiaGuuid("net", "packet", "query_block");

    public Guuid Id => PacketTypeId;

    public object GetValue(byte[] packet)
    {
        return MessagePackSerializer.Deserialize<BlockInfoPacket>(packet);
    }

    public byte[] ToPacket(object value)
    {
        Guard.IsNotNull(value);
        Guard.IsAssignableToType(value, typeof(BlockInfoPacket));
        return MessagePackSerializer.Serialize((BlockInfoPacket)value);
    }
}

