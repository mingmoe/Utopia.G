//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//

using MessagePack;

namespace Utopia.Core.Map;

/// <summary>
/// 平面位置
/// </summary>
[MessagePackObject]
public readonly struct FlatPosition
{
    [Key(0)]
    public readonly int X;
    [Key(1)]
    public readonly int Y;

    public FlatPosition(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }
}

/// <summary>
/// 三维位置
/// </summary>
[MessagePackObject]
public readonly struct Position
{
    [Key(0)]
    public readonly int X;
    [Key(1)]
    public readonly int Y;
    [Key(2)]
    public readonly int Z;

    public Position(int x, int y, int z)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
    }

    public FlatPosition ToFlat()
    {
        return new FlatPosition(X, Y);
    }
}

/// <summary>
/// 世界位置
/// </summary>
[MessagePackObject]
public readonly struct WorldPosition
{
    [Key(0)]
    public readonly int X;
    [Key(1)]
    public readonly int Y;
    [Key(2)]
    public readonly int Z;
    /// <summary>
    /// stand for the World ID
    /// </summary>
    [Key(3)]
    public readonly long Id;

    public WorldPosition(int x, int y, int z, long id)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
        this.Id = id;
    }

    public FlatPosition ToFlat()
    {
        return new FlatPosition(X, Y);
    }

    public Position ToPos()
    {
        return new Position(X, Y, Z);
    }
}

[MessagePackObject]
public readonly struct FlatPositionWithId
{
    [Key(0)]
    public readonly int X;
    [Key(1)]
    public readonly int Y;
    [Key(2)]
    public readonly long Id;

    public FlatPositionWithId(int x, int y, long id)
    {
        this.X = x;
        this.Y = y;
        this.Id = id;
    }

    public FlatPosition ToFlat()
    {
        return new FlatPosition(X, Y);
    }
}
