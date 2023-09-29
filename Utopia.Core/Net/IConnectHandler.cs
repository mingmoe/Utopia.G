using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utopia.Core.Net;
using CommunityToolkit.Diagnostics;
using NLog;
using System.Net.Sockets;
using System.Net;
using Utopia.Core.Net.Packet;
using Utopia.Core.Utilities;

namespace Utopia.G.Net;

/// <summary>
/// 链接持有器.
/// </summary>
public interface IConnectHandler : IDisposable
{
    /// <summary>
    /// 包分发器
    /// </summary>
    IDispatcher Dispatcher { get; }

    /// <summary>
    /// 包格式化器
    /// </summary>
    Packetizer Packetizer { get; }

    void Write(byte[] bytes);

    void WritePacket(Guuid packetTypeId, byte[] obj);

    /// <summary>
    /// 服务端进行信息循环
    /// </summary>
    Task InputLoop();

    void Disconnect();
}
