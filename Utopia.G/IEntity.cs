//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using Godot;

namespace Utopia.G;

/// <summary>
/// 客户端实体
/// </summary>
public interface IEntity
{
    /// <summary>
    /// 每帧调用
    /// </summary>
    /// <param name="deltaSecond">距离上次调用经过的秒数</param>
    /// <param name="node">渲染此实体的节点，通常是Sprite2D</param>
    void FrameUpdate(double deltaSecond, Sprite2D node);
}

/// <summary>
/// 简单实体类
/// </summary>
public abstract class SimplayEntity : IEntity
{
    public void FrameUpdate(double deltaSecond, Sprite2D node)
    {
        return;
    }
}
