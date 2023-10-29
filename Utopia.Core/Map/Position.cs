// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

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
        X = x;
        Y = y;
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
        X = x;
        Y = y;
        Z = z;
    }

    public FlatPosition ToFlat() => new(X, Y);
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
        X = x;
        Y = y;
        Z = z;
        Id = id;
    }

    public FlatPosition ToFlat() => new(X, Y);

    public Position ToPos() => new(X, Y, Z);
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
        X = x;
        Y = y;
        Id = id;
    }

    public FlatPosition ToFlat() => new(X, Y);
}
