// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

namespace Utopia.Core.Plugin;

/// <summary>
/// The lifecycle of a plugin.
/// </summary>
public enum PluginLifeCycle
{
    /// <summary>
    /// The instance of the plugin was constructed but the <see cref="IPluginBase.Activate"/> has not been called.
    /// This is the initial status of a plugin instance.
    /// We not ensure that the dependencies of this plugin was constructed when constructing this.
    /// </summary>
    Created,
    /// <summary>
    /// Called the <see cref="IPluginBase.Activate"/> but <see cref="IPluginBase.Deactivate"/> has not been called.
    /// We ensure that the dependencies of this plugin was activated when activating this.
    /// </summary>
    Activated,
    /// <summary>
    /// <see cref="IPluginBase.Activate"/> and <see cref="IPluginBase.Deactivate"/> all have been called.
    /// We ensure that the dependencies of this plugin was deactivated when deactivating this.
    /// </summary>
    Deactivated,
    // TODO: ensure we activate/deactivate dependencies before activate/deactivate this
}

