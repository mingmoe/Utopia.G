#region copyright
// This file(may named Position.cs) is a part of the project: Utopia.Core.
// 
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// 
// This file is part of Utopia.Core.
//
// Utopia.Core is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Utopia.Core is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with Utopia.Core. If not, see <https://www.gnu.org/licenses/>.
#endregion

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
        return new FlatPosition(this.X, this.Y);
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
        return new FlatPosition(this.X, this.Y);
    }

    public Position ToPos()
    {
        return new Position(this.X, this.Y, this.Z);
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
        return new FlatPosition(this.X, this.Y);
    }
}
