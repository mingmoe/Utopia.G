//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using Autofac;
using CommunityToolkit.Diagnostics;
using System.Reflection;
using Utopia.Core;

namespace Utopia.Server;

/// <summary>
/// 插件加载器
/// </summary>
public class PluginLoader<PluginT> : IPluginLoader<PluginT>
{
    readonly object _locker = new();

    List<(Type, PluginT)> _ActivePlugins { get; } = new();

    public IReadOnlyCollection<(Type, PluginT)> ActivePlugins
    {
        get
        {
            lock (this._locker)
            {
                return this._ActivePlugins.ToArray();
            }
        }
    }

    public IEventManager<IPluginLoader<PluginT>.ActivePlguinEventArgs> ActivePlguinEvent { get; }
    = new EventManager<IPluginLoader<PluginT>.ActivePlguinEventArgs>();

    public void Active(IContainer container, string dllFile)
    {
        ArgumentNullException.ThrowIfNull(dllFile, nameof(dllFile));
        ArgumentNullException.ThrowIfNull(container, nameof(container));

        var loaded = Assembly.LoadFrom(dllFile);
        var types = loaded.ExportedTypes;

        foreach (var type in types)
        {
            this.Active(container, type);
        }
    }

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
            ActivePlguinEvent.Fire(args);

            if (args.PluginInstance == null)
            {
                return;
            }

            p = args.PluginInstance;

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
