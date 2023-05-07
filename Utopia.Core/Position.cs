//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//

namespace Utopia.Core;

/// <summary>
/// 平面位置
/// </summary>
public struct FlatPosition
{
    public long X;
    public long Y;
}

/// <summary>
/// 三维位置
/// </summary>
public struct Position
{
    public long X;
    public long Y;
    public long Z;

    public FlatPosition ToFlat()
    {
        return new FlatPosition
        {
            X = X,
            Y = Y
        };
    }
}

/// <summary>
/// 世界位置
/// </summary>
public struct WorldPosition
{
    public long X;
    public long Y;
    public long Z;

    public FlatPosition ToFlat()
    {
        return new FlatPosition
        {
            X = X,
            Y = Y
        };
    }

    public Position ToPos()
    {
        return new Position
        {
            X = X,
            Y = Y,
            Z = Z
        };
    }

    /// <summary>
    /// stand for the World ID
    /// </summary>
    public long Id;
}
