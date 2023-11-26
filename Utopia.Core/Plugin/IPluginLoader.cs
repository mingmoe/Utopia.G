// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Collections.Immutable;
using System.Reflection;
using Autofac;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Logging;
using Utopia.Core.Events;

namespace Utopia.Core.Plugin;

/// <summary>
/// 插件加载器接口，保证线程安全
/// </summary>
public interface IPluginLoader<PluginT> : IDisposable
{
    /// <summary>
    /// 已经实例化的插件.
    /// Why we have a `Type` ?
    /// Because the <see cref="ActivePluginEvent"/> may change the instance of the plugin.
    /// And the `Type` is the argument of the method as <see cref="Register(ContainerBuilder, Type)"/>,
    /// <see cref="AddUnresolved(Type)"/> and <see cref="AddResolved(PluginT, Type?)"/>.
    /// <see cref="ActivePluginEventArgs.PluginType"/> can change the `Type`.
    /// In sum,the `Type` won't always equal to <see cref="object.GetType()"/>) of the instance.
    /// </summary>
    ImmutableArray<(Type, PluginT)> ActivatedPlugins { get; }

    /// <summary>
    /// Unsolved plugins. Call <see cref="IPluginLoader{PluginT}.Active(IContainer)"/>
    /// should remove all of them,and create instances of each them,add them into
    /// <see cref="PluginLoader{PluginT}.ActivatedPlugins"/>.
    /// </summary>
    ImmutableArray<Type> UnresolvePlugins { get; }

    /// <summary>
    /// 激活插件时触发.
    /// </summary>
    public class ActivePluginEventArgs : Event
    {
        /// <summary>
        /// 插件类型. If it's null(and <see cref="PluginInstance"/> is not null), throw a <see cref="Exceptions.EventAssertionException"/>.
        /// </summary>
        public Type PluginType { get; set; }

        /// <summary>
        /// 如果在事件<see cref="IPluginLoader{PluginT}.ActivePluginEvent"/> after firing时此值为null，那么则将会取消对这个插件的加载。
        /// 默认将会有一个使用默认容器对插件进行依赖注入生成的实例。
        /// </summary>
        public PluginT? PluginInstance { get; set; }

        /// <summary>
        /// 默认的容器,if any.
        /// <see cref="IPluginLoader{PluginT}.AddResolved(PluginT)"/>
        /// </summary>
        public IContainer? Container { get; }

        public ActivePluginEventArgs(PluginT plugin, Type pluginType, IContainer? container)
        {
            Guard.IsNotNull(pluginType);
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
    /// Register plugin from dll file。将会注册所有实现了<see cref="PluginT"/>的类型。
    /// </summary>
    /// <param name="dllFile">dll文件</param>
    void RegisterPluginFromDll(string dllFile)
    {
        ArgumentNullException.ThrowIfNull(dllFile, nameof(dllFile));

        var loaded = Assembly.LoadFrom(dllFile);
        IEnumerable<Type> types = loaded.ExportedTypes;

        foreach (Type type in types)
        {
            Register(type);
        }
    }

    /// <summary>
    /// Active all types of plugin in <see cref="IPluginLoader{PluginT}.UnresolvePlugins"/>.
    /// They will be resolved using the IContainer from the argument. And then fire the <see cref="ActivePluginEvent"/>,
    /// and then call the <see cref="IPluginBase.Activate"/> method of the plugin.
    /// If a plugin throw any exception when call <see cref="IPluginBase.Activate"/>,
    /// this plugin will be ignored and won't be added to the <see cref="ActivatedPlugins"/>.
    /// </summary>
    /// <param name="container">This container should from the argument of method
    /// <see cref="Register(ContainerBuilder, Type)"/>
    /// and <see cref="Register(ContainerBuilder, Type)"/></param>
    /// <param name="type">插件类型，要求实现<see cref="PluginT"/>接口</param>
    void Active(IContainer container);

    /// <summary>
    /// This method should add type to <see cref="IPluginLoader{PluginT}.UnresolvePlugins"/>
    /// so that them can be activated.
    /// </summary>
    /// <param name="type"></param>
    void AddUnresolved(Type type);

    /// <summary>
    /// If you create a plugin manually,use this method add it to the <see cref="ActivatedPlugins"/>.
    /// This will also fire the <see cref="ActivePluginEvent"/> event.And call the <see cref="IPluginBase.Activate"/>
    /// </summary>
    /// <param name="typed">if this is null, get type from plugin.<see cref="object.GetType"/></param>
    void AddResolved(PluginT plugin, Type? typed = null);

    /// <summary>
    /// If you register a type of plugin,it will be registered in <see cref="ContainerBuilder"/>,
    /// and will be sent to <see cref="IPluginLoader{PluginT}.AddUnresolved(Type)"/>.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="type"></param>
    void Register(Type type)
    {
        AddUnresolved(type);
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
    public static void LoadFromDirectory<T>(this IPluginLoader<T> loader, string dir,ILogger logger)
    {
        foreach (string f in Directory.GetFiles(dir, "*.dll", SearchOption.AllDirectories))
        {
            string file = Path.GetFullPath(f);
            logger.LogInformation("Load plugin from dll:{dll}", file);
            loader.RegisterPluginFromDll(file);
        }
    }
}
