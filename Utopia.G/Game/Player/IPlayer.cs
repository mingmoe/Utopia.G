#region copyright
// This file(may named IPlayer.cs) is a part of the project: Utopia.G.
// 
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// 
// This file is part of Utopia.G.
//
// Utopia.G is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Utopia.G is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with Utopia.G. If not, see <https://www.gnu.org/licenses/>.
#endregion

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
        this.Name = new VariableEvent<string>(name);
        this.Position = new VariableEvent<WorldPosition>(position);
    }

    public IVariableEvent<string> Name { get; init; }

    public IVariableEvent<WorldPosition> Position { get; init; }
}
