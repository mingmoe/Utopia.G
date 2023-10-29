// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using CommunityToolkit.Diagnostics;
using MessagePack;
using Utopia.Core.Map;
using Utopia.Core.Utilities;

namespace Utopia.Core.Net.Packet;

/// <summary>
/// 查询地图包
/// </summary>
[MessagePackObject]
public class QueryBlockPacket
{
    [Key(0)]
    public WorldPosition QueryPosition { get; set; }
}

public class QueryBlockPacketFormatter : IPacketFormatter
{
    public static readonly Guuid PacketTypeId = Guuid.NewUtopiaGuuid("net", "packet", "query_block");

    public Guuid Id => PacketTypeId;

    public object GetValue(byte[] packet) => MessagePackSerializer.Deserialize<QueryBlockPacket>(packet);

    public byte[] ToPacket(object value)
    {
        Guard.IsNotNull(value);
        Guard.IsAssignableToType(value, typeof(QueryBlockPacket));
        return MessagePackSerializer.Serialize((QueryBlockPacket)value);
    }
}
