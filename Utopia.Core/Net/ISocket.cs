//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//

namespace Utopia.Core.Net
{
    /// <summary>
    /// 套接字接口
    /// </summary>
    public interface ISocket
    {
        /// <summary>
        /// 读取套接字
        /// </summary>
        /// <param name="output">输出结果</param>
        /// <returns>读取到的字节数量</returns>
        Task<int> Read(Memory<byte> output);

        /// <summary>
        /// 写入套接字
        /// </summary>
        /// <param name="data">将写入的数据</param>
        Task Write(Memory<byte> data)
        {
            return Write(data, 0, data.Length);
        }

        /// <summary>
        /// 写入套接字
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="start">开始index</param>
        Task Write(Memory<byte> data, int start, int length);

        /// <summary>
        /// 刷新套接字
        /// </summary>
        void Flush();

        /// <summary>
        /// 关闭套接字
        /// </summary>
        void Close();

        /// <summary>
        /// 套接字地址，人类可读
        /// </summary>
        string SocketAddress { get; }

        /// <summary>
        /// 链接是否存在
        /// </summary>
        bool Connected { get; }
    }
}
