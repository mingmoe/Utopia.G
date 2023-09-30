#region copyright
// This file(may named IPluginLoader.cs) is a part of the project: Utopia.Core.
// 
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// 
// This file is part of Utopia.Core.
//
// Utopia.Core is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Utopia.Core is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with Utopia.Core. If not, see <https://www.gnu.org/licenses/>.
#endregion

using Autofac;
using CommunityToolkit.Diagnostics;
using NLog;
using System.Collections.Immutable;
using System.Reflection;
using Utopia.Core.Events;

namespace Utopia.Server;

/// <summary>
/// 插件加载器接口，保证线程安全
/// </summary>
public interface IPluginLoader<PluginT>
{
    /// <summary>
    /// 已经实例化的插件
    /// </summary>
    ImmutableArray<(Type, PluginT)> ActivedPlugins { get; }

    /// <summary>
    /// Unresolverd plugins. Call <see cref="IPluginLoader{PluginT}.Active(IContainer)"/>
    /// should remove all of them,and create instances of each them,add them into
    /// <see cref="PluginLoader{PluginT}.ActivedPlugins"/>.
    /// </summary>
    ImmutableArray<Type> UnresolveredPlugins { get; }

    /// <summary>
    /// 激活插件时触发.
    /// </summary>
    public class ActivePlguinEventArgs : Event
    {
        /// <summary>
        /// 插件类型
        /// </summary>
        public Type PluginType { get; set; }

        /// <summary>
        /// 如果在事件<see cref="IPluginLoader{PluginT}.ActivePlguinEvent"/>退出时此值为null，那么则将会取消对这个插件的加载。
        /// 默认将会有一个使用默认容器对插件进行依赖注入生成的实例。
        /// </summary>
        public PluginT? PluginInstance { get; set; } = default;

        /// <summary>
        /// 默认的容器
        /// </summary>
        public IContainer Container { get; }

        public ActivePlguinEventArgs(Type pluginType, IContainer container)
        {
            Guard.IsNotNull(pluginType);
            Guard.IsNotNull(container);
            this.PluginType = pluginType;
            this.Container = container;
        }
    }

    /// <summary>
    /// 激活插件事件。<see cref="IPluginLoader{PluginT}.ActivePlguinEventArgs"/>。
    /// </summary>
    IEventManager<ActivePlguinEventArgs> ActivePlguinEvent { get; }

    /// <summary>
    /// Register plugin from dll file。将会注册所有实现了<see cref="PluginT"/>的类型。
    /// </summary>
    /// <param name="dllFile">dll文件</param>
    void RegisterPluginFromDll(ContainerBuilder builder, string dllFile)
    {
        ArgumentNullException.ThrowIfNull(dllFile, nameof(dllFile));
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));

        var loaded = Assembly.LoadFrom(dllFile);
        var types = loaded.ExportedTypes;

        foreach (var type in types)
        {
            this.Register(builder, type);
        }
    }

    /// <summary>
    /// Active all types of plugin in <see cref="IPluginLoader{PluginT}.UnresolveredPlugins"/>.
    /// </summary>
    /// <param name="container">This container should from the argument of method
    /// <see cref="Register(ContainerBuilder, Type)"/>
    /// and <see cref="Register(ContainerBuilder, Type)"/></param>
    /// <param name="type">插件类型，要求实现<see cref="PluginT"/>接口</param>
    void Active(IContainer container);

    /// <summary>
    /// This method should add type to <see cref="IPluginLoader{PluginT}.UnresolveredPlugins"/>
    /// so that them can be actived.
    /// </summary>
    /// <param name="type"></param>
    void AddUnresolved(Type type);

    /// <summary>
    /// If you register a type of plugin,it will be registered in <see cref="ContainerBuilder"/>,
    /// and will be sent to <see cref="IPluginLoader{PluginT}.AddUnresolved(Type)"/>.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="type"></param>
    void Register(ContainerBuilder builder, Type type)
    {
        builder.RegisterType(type);
        this.AddUnresolved(type);
    }
}

public static class PluginLoadHelper
{
    /// <summary>
    /// 从目录中递归搜索所有dll然后加载
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="loader"></param>
    /// <param name="dir"></param>
    /// <param name="container"></param>
    /// <param name="logger"></param>
    public static void LoadFromDirectory<T>(this IPluginLoader<T> loader, string dir,
        ContainerBuilder builder, ILogger logger)
    {
        foreach (var f in Directory.GetFiles(dir, "*.dll", SearchOption.AllDirectories))
        {
            var file = Path.GetFullPath(f);
            logger.Info("loading plugin from dll:{plugin}", file);
            loader.RegisterPluginFromDll(builder,file);
        }
    }
}
