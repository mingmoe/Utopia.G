using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utopia.Core.Utilities;
using Utopia.Server.Map;

namespace Utopia.Server.Plugin.Map;

/// <summary>
/// 世界工厂
/// </summary>
public class WorldFactory : IWorldFactory
{
    public Guuid WorldType => IDs.WorldType;

    public IWorld GenerateNewWorld()
    {
        return new World(0, 4, 4, new Generator());
    }
}
