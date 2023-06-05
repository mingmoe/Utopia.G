using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utopia.Core;
using Utopia.Core.Translate;

namespace Utopia.Server;

public interface IPlayer : IEntity
{
    /// <summary>
    /// 玩家位置
    /// </summary>
    WorldPosition PlayerPosition { get; }

    /// <summary>
    /// 玩家的客户端
    /// </summary>
    IClient Client { get; }
}

public class Player : IPlayer
{
    public WorldPosition PlayerPosition => throw new NotImplementedException();

    public IClient Client => throw new NotImplementedException();

    public TranslateKey Name => throw new NotImplementedException();

    public bool Accessible => throw new NotImplementedException();

    public bool Collidable => throw new NotImplementedException();

    public Guuid Id => new Guuid("utopia", "core", "player");

    public void LogicUpdate()
    {

    }
}

