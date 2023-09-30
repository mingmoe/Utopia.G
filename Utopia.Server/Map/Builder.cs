#region copyright
// This file(may named Builder.cs) is a part of the project: Utopia.Server.
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
using Utopia.Server.Plugin.Map;

namespace Utopia.Server.Map;

public class AreaLayerBuilder
{
    public IBlock[][] Blocks { get; init; }

    public WorldPosition Position { get; init; }

    public GenerationStage GenerationStage { get; set; }

    public AreaLayerBuilder(WorldPosition layerPosition)
    {
        this.Blocks = new IBlock[IArea.XSize][];
        this.Position = layerPosition;

        for (int i = 0; i != this.Blocks.Length; i++)
        {
            this.Blocks[IArea.XSize] = new IBlock[IArea.YSize];

            for (int j = 0; j != this.Blocks[i].Length; j++)
            {
                this.Blocks[i][j] = new Block(
                        new WorldPosition(
                            layerPosition.X + i,
                            layerPosition.Y + j,
                            layerPosition.Z,
                            layerPosition.Id
                    ));
            }
        }
    }

    public AreaLayerBuilder(IAreaLayer layer)
    {
        this.Position = layer.Position;

        this.Blocks = new IBlock[IArea.XSize][];

        for (int i = 0; i != this.Blocks.Length; i++)
        {
            this.Blocks[IArea.XSize] = new IBlock[IArea.YSize];

            for (int j = 0; j != this.Blocks[i].Length; j++)
            {
                layer.TryGetBlock(new FlatPosition(i, j), out IBlock? block);
                this.Blocks[i][j] = block ?? throw new InvalidOperationException();
            }
        }
    }

    public void ForEach(Action<IBlock, FlatPosition> action)
    {
        for (int x = 0; x != this.Blocks.Length; x++)
        {
            for (int y = 0; y != this.Blocks[x].Length; y++)
            {
                action.Invoke(this.Blocks[x][y], new FlatPosition(x, y));
            }
        }
    }

    public void Fill(Func<IBlock, FlatPosition, IEntity> entityFactory)
    {
        this.ForEach(
                (b, p) =>
                {
                    b.TryAddEntity(entityFactory.Invoke(b, p));
                });
    }

    public IBlock this[int x, int y]
    {
        get => this.Blocks[x][y];
    }

    public IAreaLayer Get()
    {
        var layer = new AreaLayer(this.Blocks, this.Position);

        return layer;
    }
}
