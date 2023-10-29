// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using Godot;
using Utopia.Core.Map;
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
    /// Render the entity,the entity can be rendered as a node or add tile to the tiemap.
    /// 
    /// </summary>
    /// <param name="position">
    /// This position stands for world position,
    /// you can use it in tilemap directly.
    /// </param>
    Node? Render(WorldPosition position, TileMap map);

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
    public Tile Tile { get; init; }

    public Guuid EntityId { get; init; }

    public abstract void FrameUpdate(double deltaSecond, Node node);

    public Node? Render(WorldPosition position, TileMap map)
    {
        map.SetCell(new Vector2I(position.X, position.Y), Tile);
        return null;
    }

    public SimplayEntity(Tile tile, Guuid id)
    {
        ArgumentNullException.ThrowIfNull(tile);
        ArgumentNullException.ThrowIfNull(id);
        Tile = tile;
        EntityId = id;
    }
}
