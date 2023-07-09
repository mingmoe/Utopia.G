using CommunityToolkit.Diagnostics;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utopia.Core.Net.Packet;

/// <summary>
/// 登录包
/// </summary>
[MessagePackObject]
public class LoginPacket
{
    [Key(0)]
    public string PlayerId { get; set; } = string.Empty;
}

public class LoginPacketFormatter : IPacketFormatter
{
    public static readonly Guuid PacketTypeId = new("utopia", "core", "net", "packet", "login");

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
