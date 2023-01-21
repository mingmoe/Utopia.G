//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//

namespace Utopia.Core
{
    /// <summary>
    /// 世界由许多Area组成。Area的X层和Y层大小固定，Z层则应该是动态生成的。
    /// 毕竟，谁也不想自己的电脑生成long.MAX_SIZE个数量的z层对象吧。
    /// </summary>
    public interface IArea
    {
        const long XSize = 32;
        const long YSize = 32;

        /// <summary>
        /// 地面的Z坐标。
        /// </summary>
        const long GroundZ = 0;

        bool TryGetBlock(Position position, out IBlock block);
    }
}
