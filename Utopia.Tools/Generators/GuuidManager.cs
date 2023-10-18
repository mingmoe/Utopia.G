using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utopia.Core.Utilities;

namespace Utopia.Tools.Generators;

/// <summary>
/// Use this class to get the guuids you need.
/// </summary>
public static class GuuidManager
{
    /// <summary>
    /// Any guuid will be used by the generator should enter the `generated` namespace.
    /// </summary>
    public static Guuid EnterGeneratedSpace(this Guuid guuid)
    {
        return guuid.Append("generated");
    }

    /// <summary>
    /// Get the translate key id of the plugin.
    /// </summary>
    public static Guuid GetTranslateGuuidOf(Guuid pluginId,Guuid translateId)
    {
        return pluginId.EnterGeneratedSpace().Append("translate").Append(translateId);
    }

    /// <summary>
    /// Get the translate provider of the plugin.
    /// </summary>
    public static Guuid GetTranslateProviderGuuidOf(Guuid pluginId)
    {
        return pluginId.EnterGeneratedSpace().Append("translate").Append("provider");
    }

    public static Guuid GetPluginNameTranslateId(Guuid pluginId)
    {
        return pluginId.EnterGeneratedSpace().Append("plugin_info","name");
    }

    public static Guuid GetPluginDescriptionTranslateId(Guuid pluginId)
    {
        return pluginId.EnterGeneratedSpace().Append("plugin_info", "description");
    }
}
