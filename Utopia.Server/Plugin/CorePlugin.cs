using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utopia.Core.Collections;
using Utopia.Core.Utilities;
using Utopia.ResourcePack;
using Utopia.Server.Map;

namespace Utopia.Server.Plugin;

public class CorePlugin : CorePluginBase
{
    private Core.IServiceProvider _Provider { get; init; }

    public CorePlugin(Core.IServiceProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider);
        this._Provider = provider;
    }

    public override void Active()
    {
        this._Provider.GetService<SafeDictionary<Guuid, IWorldFactory>>().TryAdd(
                
            );

    }
}
