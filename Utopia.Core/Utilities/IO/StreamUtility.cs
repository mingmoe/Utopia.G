#region copyright
// This file(may named StreamUtility.cs) is a part of the project: Utopia.Core.
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
using System.Net;
using System.Text;
using System;
using System.Runtime.CompilerServices;

namespace Utopia.Core.Utilities.IO;

/// <summary>
/// socket工具
/// </summary>
public static class StreamUtility
{

    /// <summary>
    /// 将会调用<see cref="IPAddress.NetworkToHostOrder(int)"/>进行处理
    /// </summary>
    /// <param name="socket"></param>
    /// <returns></returns>
    public static async Task<int> ReadIntWithEndianConver(Stream stream)
    {
        return IPAddress.NetworkToHostOrder(await ReadInt(stream));
    }

    public static async Task<int> ReadInt(Stream stream)
    {
        var buffer = await Read(stream, 4);

        return BitConverter.ToInt32(buffer);

    }

    public static async Task<byte[]> Read(Stream stream, int length)
    {
        Guard.IsNotNull(stream);

        byte[] buffer = new byte[length];
        int ptr = 0;
        while (ptr != length)
        {
            ptr += await stream.ReadAsync(new ArraySegment<byte>(buffer, ptr, buffer.Length - ptr));
        }

        return buffer;
    }

    /// <summary>
    /// It will read a length then read the data
    /// 将调用<see cref="ReadIntWithEndianConver"/>,
    /// 即<see cref="IPAddress.NetworkToHostOrder"/>
    /// </summary>
    /// <returns></returns>
    public static async Task<byte[]> ReadPacket(Stream stream)
    {
        Guard.IsNotNull(stream);

        var l = await ReadIntWithEndianConver(stream);
        return await Read(stream, l);
    }

    /// <summary>
    /// 将会进行端序转换.
    /// 即调用<see cref="ReadIntWithEndianConver"/>
    /// </summary>
    public static async Task<string> ReadString(Stream stream)
    {
        Guard.IsNotNull(stream);

        var l = await ReadIntWithEndianConver(stream);
        return Encoding.UTF8.GetString(await Read(stream, l));
    }

    /// <summary>
    /// 将会进行端序转换.
    /// 即调用<see cref="ReadIntWithEndianConver"/>
    /// </summary>
    public static async Task<Guuid> ReadGuuid(Stream stream)
    {
        return Guuid.ParseString(await ReadString(stream));
    }

    /// <summary>
    /// This will call <see cref="IPAddress.HostToNetworkOrder(int)"/> method to covert the value.
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static async Task WriteInt(Stream stream,int value)
    {
        var bytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(value));

        await stream.WriteAsync(bytes);
    }

    /// <summary>
    /// will call <see cref="WriteInt"/> to write length.
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static async Task WriteStringWithLength(Stream stream,string value)
    {
        await WriteInt(stream,value.Length);

        await stream.WriteAsync(Encoding.UTF8.GetBytes(value));

    }
}

