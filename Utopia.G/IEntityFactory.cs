//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//

namespace Utopia.G
{
    /// <summary>
    /// 实体工厂接口
    /// </summary>
    public interface IEntityFactory
    {
        /// <summary>
        /// 生产一个实体
        /// </summary>
        /// <param name="data">实体数据，通常是服务端发送的</param>
        /// <returns>实体</returns>
        IEntity Create(byte[]? data);
    }
}
