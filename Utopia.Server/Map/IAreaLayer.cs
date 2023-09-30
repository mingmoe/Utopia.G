#region copyright
// This file(may named IAreaLayer.cs) is a part of the project: Utopia.Server.
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

using Utopia.Core.Map;

namespace Utopia.Server.Map;

/// <summary>
/// 代表一个Z轴下的Area
/// </summary>
public interface IAreaLayer : Logic.IUpdatable
{
    bool TryGetBlock(FlatPosition position, out IBlock? block);

    /// <summary>
    /// Area坐标
    /// </summary>
    WorldPosition Position { get; }

    /// <summary>
    /// 标识目前区域所处的阶段
    /// </summary>
    GenerationStage Stage { get; set; }

    byte[] Save();
}
