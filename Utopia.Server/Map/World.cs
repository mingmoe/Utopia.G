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
using Utopia.Server.Logic;

namespace Utopia.Server.Map;

public class World : IWorld
{
    readonly Area[][] _areas;

    public World(long id, long xSize, long ySize)
    {
        this.Id = id;
        this.XAreaCount = xSize;
        this.XAreaNegativeCount = xSize;
        this.YAreaCount = ySize;
        this.YAreaNegativeCount = ySize;

        this._areas = new Area[xSize * 2][];

        for (long xIndex = -xSize, xAbs = 0; xIndex != xSize; xIndex++, xAbs++)
        {
            this._areas[xAbs] = new Area[ySize * 2];

            for (long yIndex = -ySize,yAbs = 0 ; yIndex != ySize; yIndex++, yAbs++)
            {
                this._areas[xAbs][yAbs] = new Area(new FlatPositionWithId
                {
                    Id = id,
                    X = xIndex * IArea.YSize,
                    Y = yIndex * IArea.XSize
                });
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
        if (position.X >= this.XAreaCount * IArea.XSize || position.X < -this.XAreaNegativeCount * IArea.XSize
           || position.Y >= this.YAreaCount * IArea.YSize || position.Y < -this.YAreaNegativeCount * IArea.YSize)
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
            posInArea = originIndex % split;
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
        this._InRange(position.ToFlat());

        var (xArea, xIndex) = _GetPosInArea(position.X, IArea.XSize);
        var (yArea, yIndex) = _GetPosInArea(position.Y, IArea.YSize);

        var area = this._areas[xArea + this.XAreaCount][yArea + this.YAreaCount];
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
