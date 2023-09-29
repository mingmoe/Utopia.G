//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using Autofac;
using Castle.Core.Logging;
using CommunityToolkit.Diagnostics;
using NLog;
using System.Collections.Immutable;
using System.Reflection;
using Utopia.Core;
using Utopia.Core.Events;

namespace Utopia.Server;

/// <summary>
/// 插件加载器
/// </summary>
public class PluginLoader<PluginT> : IPluginLoader<PluginT> where PluginT : IPluginBase
{
    readonly object _locker = new();

    List<(Type, PluginT)> _ActivePlugins { get; } = new();

    readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public ImmutableArray<(Type, PluginT)> ActivePlugins
    {
        get
        {
            lock (this._locker)
            {
                return this._ActivePlugins.ToImmutableArray();
            }
        }
    }

    public IEventManager<IPluginLoader<PluginT>.ActivePlguinEventArgs> ActivePlguinEvent { get; }
    = new EventManager<IPluginLoader<PluginT>.ActivePlguinEventArgs>();

    public void Active(IContainer container, Type type)
    {
        ArgumentNullException.ThrowIfNull(container, nameof(container));
        ArgumentNullException.ThrowIfNull(type, nameof(type));

        if (type.IsAssignableTo(typeof(PluginT)))
        {
            var p = (PluginT)container.Resolve(type);
            var args = new IPluginLoader<PluginT>.ActivePlguinEventArgs(type, container)
            {
                PluginInstance = p
            };
            this.ActivePlguinEvent.Fire(args);

            if (args.PluginInstance == null)
            {
                return;
            }

            p = args.PluginInstance;

            try
            {
                p.Active();
            }
            catch (Exception e)
            {
                _logger.Error(e,
                    "failed to active plugin:`{pluginName}`(ID {pluginId}) try to access {pluginHomepage} for help",
                    p.Name,
                    p.Id,
                    p.Homepage);
            }

            lock (this._locker)
            {
                this._ActivePlugins.Add((type, p));
            }
        }
        else
        {
            throw new ArgumentException($"try to active a type without implening {typeof(PluginT)} interafce");
        }
    }
}
