//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Utopia.Core
{
    /// <summary>
    /// 游戏实体接口。
    /// 游戏实体是任何出现在地图上的可互动的“东西”。
    /// 一些视角特效等不算实体。
    /// 典型实体如：生物，玩家，掉落物，建筑。
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// 实体名称，用于显示给玩家。通常是翻译id（为了国际化）。
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 实体是否可供生物等其他实体通过。
        /// </summary>
        bool Accessible { get; set; }

        /// <summary>
        /// 实体是否可和其他可碰撞的实体进行碰撞。
        /// </summary>
        bool Collidable { get; set; }   

        /// <summary>
        /// 对于每一种实体，都需要一种Id与其对应，作为唯一标识符。
        /// </summary>
        Guuid Id { get; set; }



    }

}
