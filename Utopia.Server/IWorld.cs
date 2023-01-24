//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//

namespace Utopia.Core;

/// <summary>
/// 世界接口
/// </summary>
public interface IWorld
{
    /// <summary>
    /// 世界ID
    /// </summary>
    public long Id { get; }

    /// <summary>
    /// X轴世界大小，单位为区域
    /// </summary>
    long XAreaCount { get; }

    /// <summary>
    /// X轴世界负半轴大小，单位为区域
    /// </summary>
    long XAreaNegativeCount { get; }

    /// <summary>
    /// Y轴世界大小，单位为区域
    /// </summary>
    long YAreaCount { get; }

    /// <summary>
    /// Y轴世界负半轴大小，单位为区域
    /// </summary>
    long YAreaNegativeCount { get; }

    bool TryGetArea(FlatPosition position, out IArea? area);

    bool TryGetBlock(Position position, out IBlock? block);
}
