// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Collections.Immutable;
using System.Reflection;
using System.Text;
using Autofac;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Logging;
using Utopia.Core.Events;

namespace Utopia.Core.Plugin;

/// <summary>
/// 插件加载器接口，保证线程安全
/// </summary>
public interface IPluginLoader<PluginT> : IDisposable where PluginT:IPluginBase,IPluginInformation
{
    /// <summary>
    /// Loaded plugins
    /// </summary>
    ImmutableArray<PluginContext<PluginT>> LoadedPlugins { get; }

    /// <summary>
    /// 激活插件时触发.
    /// </summary>
    public class ActivePluginEventArgs : Event
    {
        public Type PluginType { get; }

        public PluginT? PluginInstance { get; }

        public IContainer? Container { get; }

        public ActivePluginEventArgs(PluginT plugin, Type pluginType, IContainer? container)
        {
            Guard.IsNotNull(pluginType);
            Guard.IsNotNull(plugin);
            PluginType = pluginType;
            Container = container;
            PluginInstance = plugin;
        }
    }

    /// <summary>
    /// 激活插件事件。<see cref="IPluginLoader{PluginT}.ActivePluginEventArgs"/>。
    /// </summary>
    IEventManager<ActivePluginEventArgs> ActivePluginEvent { get; }

    /// <summary>
    /// </summary>
    /// <param name="container">This container should from the argument of method
    /// <see cref="Register(ContainerBuilder, Type)"/>
    /// and <see cref="Register(ContainerBuilder, Type)"/></param>
    /// <param name="type">插件类型，要求实现<see cref="PluginT"/>接口</param>
    void ActiveAllPlugins();

    void AddPlugin(PluginContext<PluginT> loadedPlugin);
}
