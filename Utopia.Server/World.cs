//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
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

    public bool TryGetArea(FlatPosition position, out IArea? area)
    {
        if (position.X >= (this.XAreaCount * IArea.XSize) || position.X < (-this.XAreaNegativeCount * IArea.XSize)
           || position.Y >= (this.YAreaCount * IArea.YSize) || position.Y < (-this.YAreaNegativeCount * IArea.YSize))
        {
            area = null;
            return false;
        }

        static int get(in long value, in int unit)
        {
            if (value < 0)
            {
                return (int)((value - 2) / unit);
            }
            else
            {
                return (int)((value + 2) / unit);
            }
        }

        var xa = get(position.X, IArea.XSize);
        var ya = get(position.Y, IArea.YSize);

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

        static int get(in long value, in int unit)
        {
            if (value < 0)
            {
                var max = (int)Math.Abs(Math.Floor((double)value / unit));
                return (int)(value + (max * unit));
            }
            else
            {
                var min = (int)Math.Abs(Math.Floor((double)(value < 0 ? value + 1 : value) / unit));
                return (int)(value - (min == 0 ? 0 : min * unit));
            }
        }

        int xi = get(position.X, IArea.XSize);
        int yi = get(position.Y, IArea.YSize);

        area!.TryGetBlock(new Position { X = xi, Y = yi, Z = position.Z }, out block);
        return true;
    }
}
