// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using Utopia.Core;
using Utopia.Core.Map;

namespace Utopia.Server.Map;

/// <summary>
/// 代表一个图块，是地图上每个坐标的所指。
/// </summary>
public interface IBlock : ISaveable
{
    bool TryAddEntity(IEntity entity);

    void RemoveEntity(IEntity entity);

    bool Contains(IEntity entity);

    bool IsEmpty();

    long EntityCount();

    IReadOnlyCollection<IEntity> GetAllEntities();

    bool Collidable { get; }

    bool Accessable { get; }

    void LogicUpdate();

    WorldPosition Position { get; }

    /// <summary>
    /// Note:This method may break the block infomation
    /// </summary>
    /// <param name="action"></param>
    void OperateEntities(Action<IList<IEntity>> action);
}
