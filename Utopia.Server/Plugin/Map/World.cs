#region copyright
// This file(may named World.cs) is a part of the project: Utopia.Server.
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

using CommunityToolkit.Diagnostics;
using Utopia.Core.Map;
using Utopia.Core.Utilities;
using Utopia.Core.Utilities.IO;
using Utopia.Server.Logic;
using Utopia.Server.Map;

namespace Utopia.Server.Plugin.Map;

public class World : IWorld
{
    readonly Area[][] _areas;

    public World(long id, int xSize, int ySize, IWorldGenerator generator)
    {
        ArgumentNullException.ThrowIfNull(generator);
        this.Id = id;
        this.XAreaCount = xSize;
        this.XAreaNegativeCount = xSize;
        this.YAreaCount = ySize;
        this.YAreaNegativeCount = ySize;

        this._areas = new Area[xSize * 2][];

        for (int xIndex = -xSize, xAbs = 0; xIndex != xSize; xIndex++, xAbs++)
        {
            this._areas[xAbs] = new Area[ySize * 2];

            for (int yIndex = -ySize, yAbs = 0; yIndex != ySize; yIndex++, yAbs++)
            {
                this._areas[xAbs][yAbs] = new Area(new FlatPositionWithId(
                    xIndex * IArea.YSize,
                    yIndex * IArea.XSize,
                    id
                ));
            }
        }

        this.Generator = generator;
    }

    public IWorldGenerator Generator { get; init; }

    public long Id { get; init; }

    public int XAreaCount { get; init; }

    public int XAreaNegativeCount { get; init; }

    public int YAreaCount { get; init; }

    public int YAreaNegativeCount { get; init; }

    public Guuid WorldType => IDs.WorldType;

    private bool _InRange(FlatPosition position)
    {
        if (position.X >= this.XAreaCount * IArea.XSize || position.X < -this.XAreaNegativeCount * IArea.XSize
           || position.Y >= this.YAreaCount * IArea.YSize || position.Y < -this.YAreaNegativeCount * IArea.YSize)
        {
            return false;
        }
        return true;
    }

    private static (int areaIndex, int posInArea) _GetPosInArea(int originIndex, int split)
    {
        int areaIndex;
        int posInArea;
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
        area!.TryGetBlock(new Position(xIndex, yIndex, position.Z), out block);

        // 生成世界
        var layer = area.GetLayer(position.Z);

        if (layer.Stage != GenerationStage.Finish)
        {
            this.Generator.Generate(layer);
        }

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

    public void SaveTo(DirectoryInfo path)
    {
        MemoryStream stream = new();
        foreach (var x in this._areas)
        {
            foreach (var y in x)
            {
                StreamUtility.WriteInt(stream, y.Position.X).Wait();
                StreamUtility.WriteInt(stream, y.Position.Y).Wait();
                StreamUtility.WriteDataWithLength(stream, y.Save()).Wait();
            }
        }
        File.WriteAllBytes(Path.Join(path.FullName, "data.bin"), stream.ToArray());
    }
}
