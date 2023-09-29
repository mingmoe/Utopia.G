using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utopia.Core.Utilities;

namespace Utopia.Server.Map;

public interface IWorldFactory
{
    Guuid WorldType { get; }

    IWorld GenerateNewWorld();
}
