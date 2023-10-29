// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using Utopia.Core.Events;
using Utopia.Core.Map;

namespace Utopia.G.Game.Player;

/// <summary>
/// 玩家
/// </summary>
public interface IPlayer
{
    public IVariableEvent<string> Name { get; }

    public IVariableEvent<WorldPosition> Position { get; }
}

public class Player : IPlayer
{
    public Player(string name, WorldPosition position)
    {
        Name = new VariableEvent<string>(name);
        Position = new VariableEvent<WorldPosition>(position);
    }

    public IVariableEvent<string> Name { get; init; }

    public IVariableEvent<WorldPosition> Position { get; init; }
}
