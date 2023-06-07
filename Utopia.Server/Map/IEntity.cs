//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//

using Utopia.Core;
using Utopia.Core.Translate;

namespace Utopia.Server.Map;

/// <summary>
/// 游戏实体接口。
/// 游戏实体是任何出现在地图上的可互动的“东西”。
/// 一些视角特效等不算实体。
/// 典型实体如：生物，玩家，掉落物，建筑。
/// </summary>
public interface IEntity
{
    /// <summary>
    /// 实体名称，用于显示给玩家。
    /// </summary>
    TranslateKey Name { get; }

    /// <summary>
    /// 实体是否可供生物等其他实体通过。
    /// </summary>
    bool Accessible { get; }

    /// <summary>
    /// 实体是否可和其他可碰撞的实体进行碰撞。
    /// </summary>
    bool Collidable { get; }

    /// <summary>
    /// 对于每一种实体，都需要一种Id与其对应，作为唯一标识符。
    /// </summary>
    Guuid Id { get; }

    /// <summary>
    /// 每个逻辑帧调用。一秒20个逻辑帧，可能从不同线程发起调用。
    /// </summary>
    void LogicUpdate();

    /// <summary>
    /// 世界坐标。
    /// </summary>
    WorldPosition WorldPosition { get; set; }
}