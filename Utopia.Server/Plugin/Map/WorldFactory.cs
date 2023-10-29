// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using Utopia.Core.Utilities;
using Utopia.Server.Map;

namespace Utopia.Server.Plugin.Map;

/// <summary>
/// 世界工厂
/// </summary>
public class WorldFactory : IWorldFactory
{
    public Guuid WorldType => IDs.WorldType;

    private readonly Generator _generator;

    public WorldFactory(Generator generator)
    {
        ArgumentNullException.ThrowIfNull(generator, nameof(generator));
        _generator = generator;
    }

    public IWorld GenerateNewWorld() => new World(0, 4, 4, _generator);
}
