// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using Utopia.Core.Collections;
using Utopia.Core.Map;
using Utopia.Core.Utilities.IO;
using Utopia.Server.Map;

namespace Utopia.Server.Plugin.Map;

public class Area : IArea
{
    private readonly object _lock = new();
    private readonly SafeDictionary<int, AreaLayer> _floors = new();

    public FlatPositionWithId Position { get; init; }

    private ITemperature _temperature = new EmptyTemperature();

    public ITemperature Temperature
    {
        get
        {
            lock (_lock)
            {
                return _temperature;
            }
        }
        set
        {
            lock (_lock)
            {
                _temperature = value;
            }
        }
    }

    private IBiome _biome = new EmptyBiome();

    public IBiome Biome
    {
        get
        {
            lock (_lock)
            {
                return _biome;
            }
        }
        set
        {
            lock (_lock)
            {
                _biome = value;
            }
        }
    }

    private IPrecipitation _precipitation = new EmptyPrecipiation();

    public IPrecipitation Precipitation
    {
        get
        {
            lock (_lock)
            {
                return _precipitation;
            }
        }
        set
        {
            lock (_lock)
            {
                _precipitation = value;
            }
        }
    }

    private IElevation _elevation = new EmptyElevation();

    public IElevation Elevation
    {
        get
        {
            lock (_lock)
            {
                return _elevation;
            }
        }
        set
        {
            lock (_lock)
            {
                _elevation = value;
            }
        }
    }

    private IConstruction? _construction = null;

    public IConstruction? Construction
    {
        get
        {
            lock (_lock)
            {
                return _construction;
            }
        }
        set
        {
            lock (_lock)
            {
                _construction = value;
            }
        }
    }

    public Area(FlatPositionWithId worldPosition) => Position = worldPosition;

    public bool TryGetBlock(Position position, out IBlock? block)
    {
        AreaLayer floor = _floors.GetOrAdd(position.Z, (k) =>
        {
            return new AreaLayer(
                    new WorldPosition(Position.X,
                    Position.Y,
                    k,
                    Position.Id
                ));
        });

        return floor.TryGetBlock(position.ToFlat(), out block);
    }

    public IAreaLayer GetLayer(int z) => _floors.GetOrAdd(
            z, (k) =>
            {
                return new AreaLayer(
                    new WorldPosition(Position.X,
                    Position.Y,
                    k,
                    Position.Id
                ));
            });

    public void Update(Logic.IUpdater updater)
    {
        KeyValuePair<int, AreaLayer>[] floor = _floors.ToArray();

        foreach (KeyValuePair<int, AreaLayer> z in floor)
        {
            z.Value.Update(updater);
        }
    }

    public byte[] Save()
    {
        KeyValuePair<int, AreaLayer>[] floors = _floors.ToArray();
        var stream = new MemoryStream();

        foreach (KeyValuePair<int, AreaLayer> floor in floors)
        {
            StreamUtility.WriteInt(stream, floor.Key).Wait();
            StreamUtility.WriteDataWithLength(stream, floor.Value.Save()).Wait();
        }

        return stream.ToArray();
    }
}
