//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utopia.Core;

namespace Utopia.Server.Map;

public class Area : IArea
{

    readonly SafeDictionary<long, IBlock[][]> _floors = new();

    public FlatPositionWithId WorldPosition { get; init; }

    public Area(FlatPositionWithId worldPosition)
    {
        this.WorldPosition = worldPosition;
    }

    public bool TryGetBlock(Position position, out IBlock? block)
    {
        if (position.X >= IArea.XSize || position.Y >= IArea.YSize)
        {
            block = null;
            return false;
        }

        var floor = _floors.GetOrAdd(position.Z, (k) =>
        {
            var n = new Block[IArea.XSize][];

            for (var xIndex = 0; xIndex != IArea.XSize; xIndex++)
            {
                n[xIndex] = new Block[IArea.YSize];

                for (var yIndex = 0; yIndex != IArea.YSize; yIndex++)
                {
                    n[xIndex][yIndex] = new(new WorldPosition()
                    {
                        X = xIndex + this.WorldPosition.X,
                        Y = yIndex + this.WorldPosition.Y,
                        Z = position.Z,
                        Id = this.WorldPosition.Id
                    });
                }
            }
            return n;
        });

        block = floor[position.X][position.Y];

        return true;
    }

    public void Update(Logic.IUpdater updater)
    {
        var floor = _floors.ToArray();

        foreach (var z in floor)
        {
            foreach (var x in z.Value)
            {
                foreach (var y in x)
                {
                    updater.AssignTask(() => y.LogicUpdate());
                }
            }
        }
    }
}
