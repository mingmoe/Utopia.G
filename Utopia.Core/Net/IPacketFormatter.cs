using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utopia.Core.Net;

/// <summary>
/// 包格式化器
/// </summary>
public interface IPacketFormatter
{
    Guuid Id { get; }

    object GetValue(byte[] packet);

    byte[] ToPacket(object value);
}
