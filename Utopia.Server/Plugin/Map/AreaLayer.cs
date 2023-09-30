#region copyright
// This file(may named AreaLayer.cs) is a part of the project: Utopia.Server.
// 
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// 
// This file is part of Utopia.Server.
//
// Utopia.Server is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Utopia.Server is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with Utopia.Server. If not, see <https://www.gnu.org/licenses/>.
#endregion

using Utopia.Core.Map;
using Utopia.Server.Logic;
using Utopia.Server.Map;

namespace Utopia.Server.Plugin.Map;

public class AreaLayer : IAreaLayer
{
    private readonly IBlock[][] _blocks;
    private readonly object _lock = new();

    public WorldPosition Position { get; init; }

    private GenerationStage _stage = GenerationStage.NoStart;

    public GenerationStage Stage
    {
        get
        {
            lock (this._lock)
            {
                return this._stage;
            }
        }
        set
        {
            lock (this._lock)
            {
                this._stage = value;
            }
        }
    }

    internal AreaLayer(IBlock[][] blocks, WorldPosition position)
    {
        this._blocks = blocks;
        this.Position = position;
    }

    public AreaLayer(WorldPosition position)
    {
        this.Position = position;

        var n = new Block[IArea.XSize][];

        for (var xIndex = 0; xIndex != IArea.XSize; xIndex++)
        {
            n[xIndex] = new Block[IArea.YSize];

            for (var yIndex = 0; yIndex != IArea.YSize; yIndex++)
            {
                n[xIndex][yIndex] = new(new WorldPosition(
                     xIndex + this.Position.X,
                     yIndex + this.Position.Y,
                     position.Z,
                     this.Position.Id));
            }
        }

        this._blocks = n;
    }

    public bool TryGetBlock(FlatPosition position, out IBlock? block)
    {
        if (position.X <= 0 || position.X >= IArea.XSize
            || position.Y <= 0 || position.Y >= IArea.YSize)
        {
            block = null;
            return false;
        }

        block = this._blocks[position.X][position.Y];

        return true;
    }

    public void Update(IUpdater updater)
    {
        foreach (var blocks in this._blocks)
        {
            foreach (var block in blocks)
            {
                updater.AssignTask(block.LogicUpdate);
            }
        }
    }

    public byte[] Save()
    {
        MemoryStream stream = new();

        foreach (var x in this._blocks)
        {
            foreach (var y in x)
            {
                stream.Write(y.Save());
            }
        }

        return stream.ToArray();
    }
}
