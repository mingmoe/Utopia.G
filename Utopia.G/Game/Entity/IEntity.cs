//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using Godot;
using System;
using Utopia.Core.Events;
using Utopia.Core.Utilities;
using Utopia.G.Graphy;

namespace Utopia.G.Game.Entity;

/// <summary>
/// 客户端实体
/// </summary>
public interface IGodotEntity
{
    /// <summary>
    /// 这个实体id应该和服务端的实体id一样.
    /// </summary>
    Guuid EntityId { get; }

    /// <summary>
    /// 实体的贴图
    /// </summary>
    IVariableEvent<Tile> Tile { get; }

    /// <summary>
    /// 每帧调用
    /// </summary>
    /// <param name="deltaSecond">距离上次调用经过的秒数</param>
    /// <param name="node">渲染此实体的节点</param>
    void FrameUpdate(double deltaSecond, Node node);
}

/// <summary>
/// 简单实体类
/// </summary>
public abstract class SimplayEntity : IGodotEntity
{

    public IVariableEvent<Tile> Tile { get; init; }

    public Guuid EntityId { get; init; }

    public abstract void FrameUpdate(double deltaSecond, Node node);

    public SimplayEntity(Tile tile, Guuid id)
    {
        ArgumentNullException.ThrowIfNull(tile);
        ArgumentNullException.ThrowIfNull(id);
        this.Tile = new VariableEvent<Tile>(tile);
        this.EntityId = id;
    }
}
