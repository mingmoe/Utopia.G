#region copyright
// This file(may named IConnectHandler.cs) is a part of the project: Utopia.Core.
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

using Utopia.Core.Exceptions;
using Utopia.Core.Net;
using Utopia.Core.Utilities;

namespace Utopia.G.Net;

/// <summary>
/// The class will process the read and write of the socket.
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
    IPacketizer Packetizer { get; }

    void Write(byte[] bytes);

    /// <summary>
    /// Write a packet,but it wont covert the packet to bytes.You should do it first.
    /// Or see <see cref="WritePacket(Guuid, object)"/>.
    /// </summary>
    /// <param name="packetTypeId"></param>
    /// <param name="obj"></param>
    void WritePacket(Guuid packetTypeId, byte[] obj);

    void WritePacket(Guuid packetTypeId, object packetObject)
    {
        if (this.Packetizer.TryGetFormatter(packetTypeId, out var formatter))
        {
            var bytes = formatter.ToPacket(packetObject);

            this.WritePacket(packetTypeId, bytes);

            return;
        }
        throw new FormatterNotFoundExceptiom(packetTypeId);
    }

    /// <summary>
    /// 服务端进行信息循环
    /// </summary>
    Task InputLoop();

    void Disconnect();
}
