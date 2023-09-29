//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//

using Utopia.Core.Map;
using Utopia.Core.Utilities;
using Utopia.Server;

namespace Utopia.Server.Map;

/// <summary>
/// 世界接口
/// </summary>
public interface IWorld : Logic.IUpdatable
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
