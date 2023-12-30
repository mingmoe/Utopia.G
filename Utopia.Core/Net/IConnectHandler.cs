// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using Utopia.Core.Events;
using Utopia.Core.Exceptions;
using Utopia.Core.Utilities;

namespace Utopia.Core.Net;

/// <summary>
/// The class will process the read and write of the socket.
/// </summary>
public interface IConnectHandler : IDisposable
{
    bool Running { get; }

    /// <summary>
    /// 包分发器
    /// </summary>
    IDispatcher Dispatcher { get; }

    /// <summary>
    /// 包格式化器
    /// </summary>
    IPacketizer Packetizer { get; }

    /// <summary>
    /// Write a packet,but it wont covert the packet to bytes.You should do it first.
    /// Or see <see cref="WritePacket(Guuid, object)"/>.
    /// </summary>
    /// <param name="packetTypeId"></param>
    /// <param name="obj"></param>
    void WritePacket(Guuid packetTypeId, object obj);

    /// <summary>
    /// 服务端进行信息循环
    /// </summary>
    Task InputLoop();

    void Disconnect();

    IEventManager<IEventWithParam<Exception?>> SocketDisconnectEvent { get; }
}
