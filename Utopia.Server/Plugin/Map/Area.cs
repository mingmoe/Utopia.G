#region copyright
// This file(may named Area.cs) is a part of the project: Utopia.Server.
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

using Utopia.Core.Collections;
using Utopia.Core.Map;
using Utopia.Core.Utilities.IO;
using Utopia.Server.Map;

namespace Utopia.Server.Plugin.Map;

public class Area : IArea
{
    private readonly object _lock = new();

    readonly SafeDictionary<int, AreaLayer> _floors = new();

    public FlatPositionWithId Position { get; init; }

    private ITemperature _temperature = new EmptyTemperature();

    public ITemperature Temperature
    {
        get
        {
            lock (this._lock)
            {
                return this._temperature;
            }
        }
        set
        {
            lock (this._lock)
            {
                this._temperature = value;
            }
        }
    }

    private IBiome _biome = new EmptyBiome();

    public IBiome Biome
    {
        get
        {
            lock (this._lock)
            {
                return this._biome;
            }
        }
        set
        {
            lock (this._lock)
            {
                this._biome = value;
            }
        }
    }

    private IPrecipitation _precipitation = new EmptyPrecipiation();

    public IPrecipitation Precipitation
    {
        get
        {
            lock (this._lock)
            {
                return this._precipitation;
            }
        }
        set
        {
            lock (this._lock)
            {
                this._precipitation = value;
            }
        }
    }

    private IElevation _elevation = new EmptyElevation();

    public IElevation Elevation
    {
        get
        {
            lock (this._lock)
            {
                return this._elevation;
            }
        }
        set
        {
            lock (this._lock)
            {
                this._elevation = value;
            }
        }
    }

    private IConstruction? _construction = null;

    public IConstruction? Construction
    {
        get
        {
            lock (this._lock)
            {
                return this._construction;
            }
        }
        set
        {
            lock (this._lock)
            {
                this._construction = value;
            }
        }
    }

    public Area(FlatPositionWithId worldPosition)
    {
        this.Position = worldPosition;
    }

    public bool TryGetBlock(Position position, out IBlock? block)
    {
        var floor = this._floors.GetOrAdd(position.Z, (k) =>
        {
            return new AreaLayer(
                    new WorldPosition(this.Position.X,
                    this.Position.Y,
                    k,
                    this.Position.Id
                ));
        });

        return floor.TryGetBlock(position.ToFlat(), out block);
    }

    public IAreaLayer GetLayer(int z)
    {
        return this._floors.GetOrAdd(
            z, (k) =>
            {
                return new AreaLayer(
                    new WorldPosition(this.Position.X,
                    this.Position.Y,
                    k,
                    this.Position.Id
                ));
            });
    }

    public void Update(Logic.IUpdater updater)
    {
        var floor = this._floors.ToArray();

        foreach (var z in floor)
        {
            z.Value.Update(updater);
        }
    }

    public byte[] Save()
    {
        var floors = this._floors.ToArray();
        var stream = new MemoryStream();

        foreach(var floor in floors)
        {
            StreamUtility.WriteInt(stream,floor.Key).Wait();
            StreamUtility.WriteDataWithLength(stream, floor.Value.Save()).Wait();
        }

        return stream.ToArray();
    }
}
