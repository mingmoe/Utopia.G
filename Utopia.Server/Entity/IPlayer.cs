using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utopia.G.Net;
using Utopia.Server.Map;

namespace Utopia.Server.Entity;

public interface IPlayer : IEntity
{
    public IConnectHandler ConnectHandler { get; }
}