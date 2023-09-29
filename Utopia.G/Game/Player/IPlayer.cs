using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
