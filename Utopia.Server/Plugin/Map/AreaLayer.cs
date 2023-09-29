using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        if(position.X <= 0 || position.X >= IArea.XSize
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
        foreach(var blocks in this._blocks)
        {
            foreach(var block in blocks)
            {
                updater.AssignTask(block.LogicUpdate);
            }
        }
    }
}
