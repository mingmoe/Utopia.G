// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using Utopia.Core.Map;
using Utopia.Core.Utilities.IO;
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
            lock (_lock)
            {
                return _stage;
            }
        }
        set
        {
            lock (_lock)
            {
                _stage = value;
            }
        }
    }

    internal AreaLayer(IBlock[][] blocks, WorldPosition position)
    {
        _blocks = blocks;
        Position = position;
    }

    public AreaLayer(WorldPosition position)
    {
        Position = position;

        var n = new Block[IArea.XSize][];

        for (int xIndex = 0; xIndex != IArea.XSize; xIndex++)
        {
            n[xIndex] = new Block[IArea.YSize];

            for (int yIndex = 0; yIndex != IArea.YSize; yIndex++)
            {
                n[xIndex][yIndex] = new(new WorldPosition(
                     xIndex + Position.X,
                     yIndex + Position.Y,
                     position.Z,
                     Position.Id));
            }
        }

        _blocks = n;
    }

    public bool TryGetBlock(FlatPosition position, out IBlock? block)
    {
        if (position.X <= 0 || position.X >= IArea.XSize
            || position.Y <= 0 || position.Y >= IArea.YSize)
        {
            block = null;
            return false;
        }

        block = _blocks[position.X][position.Y];

        return true;
    }

    public void Update(IUpdater updater)
    {
        foreach (IBlock[] blocks in _blocks)
        {
            foreach (IBlock block in blocks)
            {
                updater.AssignTask(block.LogicUpdate);
            }
        }
    }

    public byte[] Save()
    {
        MemoryStream stream = new();

        foreach (IBlock[] x in _blocks)
        {
            foreach (IBlock y in x)
            {
                StreamUtility.WriteDataWithLength(stream, y.Save()).Wait();
            }
        }

        return stream.ToArray();
    }
}
