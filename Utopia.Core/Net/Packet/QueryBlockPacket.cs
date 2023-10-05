#region copyright
// This file(may named QueryMapPacket.cs) is a part of the project: Utopia.Core.
// 
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// 
// This file is part of Utopia.Core.
//
// Utopia.Core is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Utopia.Core is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with Utopia.Core. If not, see <https://www.gnu.org/licenses/>.
#endregion

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

    public object GetValue(byte[] packet)
    {
        return MessagePackSerializer.Deserialize<QueryBlockPacket>(packet);
    }

    public byte[] ToPacket(object value)
    {
        Guard.IsNotNull(value);
        Guard.IsAssignableToType(value, typeof(QueryBlockPacket));
        return MessagePackSerializer.Serialize((QueryBlockPacket)value);
    }
}
