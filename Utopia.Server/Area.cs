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

namespace Utopia.Server;

public class Area : IArea
{

    readonly SafeDictionary<long, IBlock[][]> _floors = new();

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

            for (var s = 0; s != IArea.XSize; s++)
            {
                n[s] = new Block[IArea.YSize];

                for (var j = 0; j != IArea.YSize; j++)
                {
                    n[s][j] = new();
                }
            }
            return n;
        });

        block = floor[position.X][position.Y];

        return true;
    }

    public void Update(IUpdater updater)
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
