// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using Utopia.Core.Events;

namespace Utopia.Core.Plugin;

public interface IPluginBase : IPluginInformation
{

    PluginLifeCycle CurrentCycle { get; }

    IEventManager<LifeCycleEvent<PluginLifeCycle>> LifecycleEvent { get; }

    /// <summary>
    /// Call this when plugin will be actived and it's unactived now.
    /// </summary>
    void Activate();

    /// <summary>
    /// Unload the plugin. It should release all the resource of the plugin.(like <see cref="IDisposable.Dispose"/>).
    /// This will be called when the game will be over.It is not used to hot reload.(So this method should be called only once for one instance).
    /// </summary>
    void Deactivate();
}
