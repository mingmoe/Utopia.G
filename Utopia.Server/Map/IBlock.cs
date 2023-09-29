//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//

using Utopia.Core.Map;
using Utopia.Server;

namespace Utopia.Server.Map;

/// <summary>
/// 代表一个图块，是地图上每个坐标的所指。
/// </summary>
public interface IBlock
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
}
