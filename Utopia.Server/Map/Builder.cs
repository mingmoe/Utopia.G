using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        for(int i = 0; i != this.Blocks.Length; i++)
        {
            this.Blocks[IArea.XSize] = new IBlock[IArea.YSize];

            for(int j = 0; j != this.Blocks[i].Length; j++)
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

    public void ForEach(Action<IBlock,FlatPosition> action)
    {
        for(int x=0;x!=this.Blocks.Length;x++)
        {
            for (int y= 0; y != this.Blocks[x].Length;y++)
            {
                action.Invoke(this.Blocks[x][y],new FlatPosition(x,y));
            }
        }
    }

    public void Fill(Func<IBlock,FlatPosition,IEntity> entityFactory)
    {
        this.ForEach(
                (b,p) =>
                {
                    b.TryAddEntity(entityFactory.Invoke(b,p));
                });
    }

    public IBlock this[int x,int y]
    {
        get => this.Blocks[x][y];
    }

    public IAreaLayer Get()
    {
        var layer = new AreaLayer(this.Blocks,this.Position);

        return layer;
    }
}
