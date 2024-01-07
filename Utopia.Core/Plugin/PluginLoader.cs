// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Collections.Immutable;
using Autofac;
using Microsoft.Extensions.Logging;
using NLog;
using Utopia.Core.Events;
using Utopia.Core.Exceptions;

namespace Utopia.Core.Plugin;

/// <summary>
/// 插件加载器
/// </summary>
public class PluginLoader<PluginT> : IPluginLoader<PluginT> where PluginT : IPluginBase
{
    public required ILogger<PluginLoader<PluginT>> Logger { get; init; }

    private bool _disposed = false;
    private readonly object _locker = new();

    private List<PluginT> _LoadedPlugins { get; } = new();

    public ImmutableArray<PluginT> LoadedPlugins
    {
        get
        {
            lock (_locker)
            {
                return _LoadedPlugins.ToImmutableArray();
            }
        }
    }

    public IEventManager<IPluginLoader<PluginT>.ActivePluginEventArgs> ActivePluginEvent { get; }
    = new EventManager<IPluginLoader<PluginT>.ActivePluginEventArgs>();

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            lock (_locker)
            {
                foreach (var plugin in _LoadedPlugins)
                {
                    plugin.Deactivate();
                }
                _LoadedPlugins.Clear();
            }
        }

        _disposed = true;
    }

    public void ActiveAllPlugins()
    {
        PluginT[] plugins;
        lock (_locker)
        {
            plugins = this._LoadedPlugins.ToArray();
        }
        foreach(var plugin in plugins)
        {
            if(plugin.CurrentCycle == PluginLifeCycle.Created)
            {
                plugin.Activate();
            }
        }
    }
    public void AddPlugin(PluginT loadedPlugin)
    {
        lock (_locker)
        {
            if (_LoadedPlugins.Contains(loadedPlugin))
            {
                return;
            }
            _LoadedPlugins.Add(loadedPlugin);
        }
    }
}
