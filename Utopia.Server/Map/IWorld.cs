#region copyright
// This file(may named IWorld.cs) is a part of the project: Utopia.Server.
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
using Utopia.Core.Utilities;

namespace Utopia.Server.Map;

/// <summary>
/// 世界接口
/// </summary>
public interface IWorld : Logic.IUpdatable, ISaveableTo<DirectoryInfo>
{

    /// <summary>
    /// 世界类型
    /// </summary>
    Guuid WorldType { get; }

    /// <summary>
    /// 世界ID
    /// </summary>
    long Id { get; }

    /// <summary>
    /// X轴世界大小，单位为区域
    /// </summary>
    int XAreaCount { get; }

    /// <summary>
    /// X轴世界负半轴大小，单位为区域
    /// </summary>
    int XAreaNegativeCount { get; }

    /// <summary>
    /// Y轴世界大小，单位为区域
    /// </summary>
    int YAreaCount { get; }

    /// <summary>
    /// Y轴世界负半轴大小，单位为区域
    /// </summary>
    int YAreaNegativeCount { get; }

    bool TryGetArea(FlatPosition position, out IArea? area);

    bool TryGetBlock(Position position, out IBlock? block);

    /// <summary>
    /// 世界生成器
    /// </summary>
    IWorldGenerator Generator { get; }
}
