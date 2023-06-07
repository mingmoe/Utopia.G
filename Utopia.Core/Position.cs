//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//

using MessagePack;

namespace Utopia.Core;

/// <summary>
/// 平面位置
/// </summary>
[MessagePackObject]
public struct FlatPosition
{
    [Key(0)]
    public long X;
    [Key(1)]
    public long Y;
}

/// <summary>
/// 三维位置
/// </summary>
[MessagePackObject]
public struct Position
{
    [Key(0)]
    public long X;
    [Key(1)]
    public long Y;
    [Key(2)]
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
[MessagePackObject]
public struct WorldPosition
{
    [Key(0)]
    public long X;
    [Key(1)]
    public long Y;
    [Key(2)]
    public long Z;
    /// <summary>
    /// stand for the World ID
    /// </summary>
    [Key(3)]
    public long Id;

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
}

[MessagePackObject]
public struct FlatPositionWithId
{
    [Key(0)]
    public long X;
    [Key(1)]
    public long Y;
    [Key(2)]
    public long Id;

    public FlatPosition ToFlat()
    {
        return new FlatPosition
        {
            X = X,
            Y = Y
        };
    }
}
