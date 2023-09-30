#region copyright
// This file(may named IArea.cs) is a part of the project: Utopia.Server.
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
/// 世界由许多Area组成。Area的X层和Y层大小固定，Z层则应该是动态生成的。
/// 毕竟，谁也不想自己的电脑生成long.MAX_SIZE个数量的z层对象吧。
/// </summary>
public interface IArea : Logic.IUpdatable, ISaveable
{
    const int XSize = 32;
    const int YSize = 32;

    /// <summary>
    /// 地面的Z坐标。
    /// </summary>
    const int GroundZ = 0;

    bool TryGetBlock(Position position, out IBlock? block);

    IAreaLayer GetLayer(int z);

    /// <summary>
    /// Area坐标
    /// </summary>
    FlatPositionWithId Position { get; }

    IBiome Biome { get; set; }

    IPrecipitation Precipitation { get; set; }

    ITemperature Temperature { get; set; }

    IElevation Elevation { get; set; }

    IConstruction? Construction { get; set; }

}
