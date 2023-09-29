using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utopia.Core.Map;
using Utopia.Core.Translate;
using Utopia.Core.Utilities;
using Utopia.G.Net;

namespace Utopia.Server.Net;

public interface IPlayer : Map.IEntity
{
    /// <summary>
    /// 玩家位置
    /// </summary>
    WorldPosition PlayerPosition { get; }

    /// <summary>
    /// 玩家的客户端
    /// </summary>
    IConnectHandler Client { get; }
}

public class Player : IPlayer
{
    public static readonly Guuid PlayerEntityId = new("utopia", "core", "entity", "player");

    public WorldPosition PlayerPosition { get; set; }

    public IConnectHandler Client { get; init; }

    public TranslateKey Name { get; init; }

    public bool Accessible => false;

    public bool Collidable => true;

    public Guuid Id => PlayerEntityId;

    public WorldPosition WorldPosition { get; set; }

    public void LogicUpdate()
    {

    }

    public Player(IConnectHandler client, string playerId)
    {
        Guard.IsNotNull(client);
        this.Client = client;
        this.Name = TranslateKey.Create(playerId, "player id");
    }
}

