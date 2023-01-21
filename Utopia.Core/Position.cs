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
    /// 平面位置
    /// </summary>
    public struct FlatPosition
    {
        public long x;
        public long y;
    }

    /// <summary>
    /// 三维位置
    /// </summary>
    public struct Position
    {
        public long x;
        public long y;
        public long z;
    }

    /// <summary>
    /// 世界位置
    /// </summary>
    public struct WorldPosition
    {
        public long X;
        public long y;
        public long z;
        /// <summary>
        /// stand for the World ID
        /// </summary>
        public long id;
    }
}
