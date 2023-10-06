#region copyright
// This file(may named LoginPacket.cs) is a part of the project: Utopia.Core.
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
using Utopia.Core.Utilities;

namespace Utopia.Core.Net.Packet;

/// <summary>
/// 登录包
/// </summary>
[MessagePackObject]
public class LoginPacket
{
    [Key(0)]
    public string PlayerId { get; set; } = string.Empty;

    [Key(1)]
    public string Password { get; set; } = string.Empty;
}

public class LoginPacketFormatter : IPacketFormatter
{
    public static readonly Guuid PacketTypeId = Guuid.NewUtopiaGuuid("net", "packet", "login");

    public Guuid Id => PacketTypeId;

    public object GetValue(byte[] packet)
    {
        return MessagePackSerializer.Deserialize<LoginPacket>(packet);
    }

    public byte[] ToPacket(object value)
    {
        Guard.IsNotNull(value);
        Guard.IsAssignableToType(value, typeof(LoginPacket));
        return MessagePackSerializer.Serialize((LoginPacket)value);
    }
}
