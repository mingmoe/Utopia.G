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
/// 世界由许多Area组成。Area的X层和Y层大小固定，Z层则应该是动态生成的。
/// 毕竟，谁也不想自己的电脑生成long.MAX_SIZE个数量的z层对象吧。
/// </summary>
public interface IArea : Logic.IUpdatable
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
