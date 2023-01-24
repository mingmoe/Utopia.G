//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Utopia.Server;

/// <summary>
/// 插件加载器
/// </summary>
public class PluginLoader : IPluginLoader
{

    /// <summary>
    /// 已经加载的插件
    /// </summary>
    List<Type> _LoadedPlugins { get; } = new();

    /// <summary>
    /// 已经激活的插件
    /// </summary>
    List<(Type, IPlugin)> _ActivePlugins { get; } = new();

    IReadOnlyCollection<Type> IPluginLoader.LoadedPlugins => this._LoadedPlugins;

    IReadOnlyCollection<(Type, IPlugin)> IPluginLoader.ActivePlugins => this._ActivePlugins;

    public void Load(string dllFile)
    {
        ArgumentNullException.ThrowIfNull(dllFile, nameof(dllFile));

        var loaded = Assembly.LoadFrom(dllFile);
        var types = loaded.ExportedTypes;

        foreach (var type in types)
        {
            if (type.IsAssignableTo(typeof(IPlugin)))
            {
                this._LoadedPlugins.Add(type);
            }
        }
    }

    public void Load(Type type)
    {
        ArgumentNullException.ThrowIfNull(type, nameof(type));

        if (!type.IsAssignableTo<IPlugin>())
        {
            throw new ArgumentException("the type couldn't assignable to IPlugin", nameof(type));
        }

        this._LoadedPlugins.Add(type);
    }

    public void Active(IContainer container)
    {
        foreach (var plugin in this._LoadedPlugins)
        {
            var p = (IPlugin)container.Resolve(plugin);

            this._ActivePlugins.Add((plugin, p));
        }
    }
}
