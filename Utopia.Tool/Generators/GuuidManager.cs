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
    public static Guuid GetTranslationGuuidOf(Guuid pluginId, Guuid translateId) => pluginId.EnterGeneratedSpace().Append("Transitions").Append(translateId);

    public static Guuid GetTranslationGuuidOf(Guuid pluginId, params string[] id) => pluginId.EnterGeneratedSpace().Append("Transitions").Append(id);

    /// <summary>
    /// Get the translate provider of the plugin.
    /// </summary>
    public static Guuid GetTranslateProviderGuuidOf(Guuid pluginId) => pluginId.EnterGeneratedSpace().Append("TransitionsProvider");

    public static Guuid GetPluginNameTranslateId(Guuid pluginId) => GetTranslationGuuidOf(pluginId,"PluginInformation","Name");

    public static Guuid GetPluginDescriptionTranslateId(Guuid pluginId) => GetTranslationGuuidOf(pluginId, "PluginInformation", "Description");

    public static Guuid GetEntityNameTransitionOf(Guuid pluginId,Guuid entityId) => GetTranslationGuuidOf(pluginId, "Entity").Append(entityId).Append("Name");

    public static Guuid GetEntityDescriptionTransitionOf(Guuid pluginId, Guuid entityId) => GetTranslationGuuidOf(pluginId, "Entity").Append(entityId).Append("Description");
}
