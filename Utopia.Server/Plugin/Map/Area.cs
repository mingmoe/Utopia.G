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
using Utopia.Core.Collections;
using Utopia.Core.Map;
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

    public IBiome Biome {
        get {
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
        var floor = _floors.GetOrAdd(position.Z, (k) => {
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
        var floor = _floors.ToArray();

        foreach (var z in floor)
        {
            z.Value.Update(updater);
        }
    }
}
