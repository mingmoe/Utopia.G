using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utopia.Core;
using Utopia.Core.Translate;
using Utopia.G.Game;
using Utopia.ResourcePack;

namespace Utopia.G.Plugin;

/// <summary>
/// 核心插件
/// </summary>
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

    }
}
