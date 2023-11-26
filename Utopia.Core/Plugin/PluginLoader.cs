// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Collections.Immutable;
using Autofac;
using NLog;
using Utopia.Core.Events;
using Utopia.Core.Exceptions;

namespace Utopia.Core.Plugin;

/// <summary>
/// 插件加载器
/// </summary>
public class PluginLoader<PluginT> : IPluginLoader<PluginT> where PluginT : IPluginBase
{
    private bool _disposed = false;
    private readonly object _locker = new();

    private List<(Type, PluginT)> _ActivePlugins { get; } = new();

    private List<Type> _UnresolvedPlugins { get; } = new();

    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public ImmutableArray<(Type, PluginT)> ActivatedPlugins
    {
        get
        {
            lock (_locker)
            {
                return _ActivePlugins.ToImmutableArray();
            }
        }
    }

    public ImmutableArray<Type> UnresolvePlugins
    {
        get
        {
            lock (_locker)
            {
                return _UnresolvedPlugins.ToImmutableArray();
            }
        }
    }

    public void AddUnresolved(Type type)
    {
        ArgumentNullException.ThrowIfNull(type, nameof(type));
        lock (_locker)
        {
            _UnresolvedPlugins.Add(type);
        }
    }

    public IEventManager<IPluginLoader<PluginT>.ActivePluginEventArgs> ActivePluginEvent { get; }
    = new EventManager<IPluginLoader<PluginT>.ActivePluginEventArgs>();

    private void _AddPlugin(PluginT plugin, Type type, IContainer? container)
    {
        var args = new IPluginLoader<PluginT>.ActivePluginEventArgs(plugin, type, container);
        ActivePluginEvent.Fire(args);

        if (args.PluginInstance == null)
        {
            return;
        }
        if (args.PluginType == null)
        {
            throw new EventAssertionException(EventAssertionFailedCode.ResultIsNull);
        }

        plugin = args.PluginInstance;

        try
        {
            plugin.Activate();
        }
        catch (Exception e)
        {
            _logger.Error(e,
                "failed to active plugin:`{pluginName}`(ID {pluginId}) try to access {pluginHomepage} for help",
                plugin.Name,
                plugin.Id,
                plugin.Homepage);
            return;
        }

        lock (_locker)
        {
            _ActivePlugins.Add((args.PluginType, plugin));
        }
    }

    public void Active(IContainer container)
    {
        ArgumentNullException.ThrowIfNull(container, nameof(container));

        lock (_locker)
        {
            using (var scope = container.BeginLifetimeScope((config) =>
            {
                config.RegisterTypes([.. _UnresolvedPlugins]);
            }))
            {
                foreach (Type type in _UnresolvedPlugins)
                {
                    if (!type.IsAssignableTo(typeof(PluginT)))
                    {
                        throw new ArgumentException($"try to active a type without implementing {typeof(PluginT)} interface");
                    }
                    var p = (PluginT)scope.Resolve(type);
                    _AddPlugin(p, type, container);
                }
                _UnresolvedPlugins.Clear();
            }
        }
    }

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
                foreach ((Type, PluginT) plugin in _ActivePlugins)
                {
                    plugin.Item2.Deactivate();
                }
                _ActivePlugins.Clear();
                _UnresolvedPlugins.Clear();
            }
        }

        _disposed = true;
    }

    public void AddResolved(PluginT plugin, Type? typed = null)
    {
        ArgumentNullException.ThrowIfNull(plugin);
        _AddPlugin(plugin, typed ?? plugin.GetType(), null);
    }
}
