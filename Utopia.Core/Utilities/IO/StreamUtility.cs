using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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
    /// </summary>
    /// <returns></returns>
    public static async Task<byte[]> ReadPacket(Stream stream)
    {
        Guard.IsNotNull(stream);

        var l = await ReadIntWithEndianConver(stream);
        return await Read(stream, l);
    }

    public static async Task<string> ReadString(Stream stream)
    {
        Guard.IsNotNull(stream);

        var l = await ReadIntWithEndianConver(stream);
        return Encoding.UTF8.GetString(await Read(stream, l));
    }

    public static async Task<Guuid> ReadGuuid(Stream stream)
    {
        return Guuid.ParseString(await ReadString(stream));
    }

}
