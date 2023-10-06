#region copyright
// This file(may named IEntity.cs) is a part of the project: Utopia.G.
// 
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// 
// This file is part of Utopia.G.
//
// Utopia.G is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Utopia.G is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with Utopia.G. If not, see <https://www.gnu.org/licenses/>.
#endregion

using Godot;
using System;
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
        map.SetCell(new Vector2I(position.X, position.Y), this.Tile);
        return null;
    }

    public SimplayEntity(Tile tile, Guuid id)
    {
        ArgumentNullException.ThrowIfNull(tile);
        ArgumentNullException.ThrowIfNull(id);
        this.Tile = tile;
        this.EntityId = id;
    }
}
