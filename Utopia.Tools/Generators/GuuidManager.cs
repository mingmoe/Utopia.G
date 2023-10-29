// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

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
    public static Guuid EnterGeneratedSpace(this Guuid guuid) => guuid.Append("Generated");

    /// <summary>
    /// Get the translate key id of the plugin.
    /// </summary>
    public static Guuid GetTranslateGuuidOf(Guuid pluginId, Guuid translateId) => pluginId.EnterGeneratedSpace().Append("Translated").Append(translateId);

    /// <summary>
    /// Get the translate provider of the plugin.
    /// </summary>
    public static Guuid GetTranslateProviderGuuidOf(Guuid pluginId) => pluginId.EnterGeneratedSpace().Append("Translated").Append("Privoder");

    public static Guuid GetPluginNameTranslateId(Guuid pluginId) => pluginId.EnterGeneratedSpace().Append("PluginInfo", "Name");

    public static Guuid GetPluginDescriptionTranslateId(Guuid pluginId) => pluginId.EnterGeneratedSpace().Append("PluginInfo", "Description");
}
