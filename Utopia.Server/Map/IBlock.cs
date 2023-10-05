#region copyright
// This file(may named IBlock.cs) is a part of the project: Utopia.Server.
// 
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// 
// This file is part of Utopia.Server.
//
// Utopia.Server is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Utopia.Server is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with Utopia.Server. If not, see <https://www.gnu.org/licenses/>.
#endregion

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
