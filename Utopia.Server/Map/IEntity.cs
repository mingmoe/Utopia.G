#region copyright
// This file(may named IEntity.cs) is a part of the project: Utopia.Server.
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
using Utopia.Core.Translate;
using Utopia.Core.Utilities;

namespace Utopia.Server.Map;

/// <summary>
/// 游戏实体接口。
/// 游戏实体是任何出现在地图上的可互动的“东西”。
/// 一些视角特效等不算实体。
/// 典型实体如：生物，玩家，掉落物，建筑。
/// </summary>
public interface IEntity : ISaveable
{
    /// <summary>
    /// 实体名称，用于显示给玩家。
    /// </summary>
    ITranslatedString Name { get; }

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

    /// <summary>
    /// The data that the client should knows.
    /// This will be transmitted to the client.
    /// </summary>
    /// <returns></returns>
    byte[] ClientOnlyData();
}
