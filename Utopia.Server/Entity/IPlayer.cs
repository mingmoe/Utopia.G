using Utopia.G.Net;
using Utopia.Server.Map;

namespace Utopia.Server.Entity;

public interface IPlayer : IEntity
{
    public IConnectHandler ConnectHandler { get; }
}
