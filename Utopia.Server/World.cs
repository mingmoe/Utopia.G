//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Utopia.Core;

namespace Utopia.Server;

public class World : IWorld
{
    readonly Area[][] _areas;

    public World(long id, long x, long y)
    {
        this.Id = id;
        this.XAreaCount = x;
        this.XAreaNegativeCount = x;
        this.YAreaCount = y;
        this.YAreaNegativeCount = y;

        x *= 2;
        y *= 2;

        this._areas = new Area[x][];

        for (var i = 0; i != x; i++)
        {
            this._areas[i] = new Area[y];

            for (var j = 0; j != y; j++)
            {
                this._areas[i][j] = new Area();
            }
        }
    }

    public long Id { get; init; }

    public long XAreaCount { get; init; }

    public long XAreaNegativeCount { get; init; }

    public long YAreaCount { get; init; }

    public long YAreaNegativeCount { get; init; }

    private bool _InRange(FlatPosition position)
    {
        if (position.X >= (this.XAreaCount * IArea.XSize) || position.X < (-this.XAreaNegativeCount * IArea.XSize)
           || position.Y >= (this.YAreaCount * IArea.YSize) || position.Y < (-this.YAreaNegativeCount * IArea.YSize))
        {
            return false;
        }
        return true;
    }

    private static (long areaIndex, long posInArea) _GetPosInArea(long originIndex, long split)
    {
        long areaIndex;
        long posInArea;
        if (originIndex >= 0)
        {
            posInArea = (originIndex % split);
            areaIndex = (int)Math.Floor((double)originIndex / split);
        }
        else
        {
            originIndex = Math.Abs(originIndex);
            areaIndex = (int)-Math.Ceiling((double)originIndex / split);
            posInArea = originIndex % split == 0 ? 0 : split - (originIndex % split);
        }
        return new(areaIndex, posInArea);
    }

    public bool TryGetArea(FlatPosition position, out IArea? area)
    {
        if (!this._InRange(position))
        {
            area = null;
            return false;
        }

        var xa = _GetPosInArea(position.X, IArea.XSize).areaIndex;
        var ya = _GetPosInArea(position.Y, IArea.YSize).areaIndex;

        area = this._areas[xa + this.XAreaCount][ya + this.YAreaCount];
        return true;
    }

    public bool TryGetBlock(Position position, out IBlock? block)
    {
        if (!this.TryGetArea(position.ToFlat(), out var area))
        {
            block = null;
            return false;
        }

        var (xArea, xIndex) = _GetPosInArea(position.X, IArea.XSize);
        var (yArea, yIndex) = _GetPosInArea(position.Y, IArea.YSize);

        area = this._areas[xArea + this.XAreaCount][yArea + this.YAreaCount];
        area!.TryGetBlock(new Position { X = xIndex, Y = yIndex, Z = position.Z }, out block);
        return true;
    }

    public void Update(IUpdater updater)
    {
        Guard.IsNotNull(updater);

        foreach (var x in this._areas)
        {
            foreach (var y in x)
            {
                y.Update(updater);
            }
        }
    }
}
