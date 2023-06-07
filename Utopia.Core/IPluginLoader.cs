//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using Autofac;
using CommunityToolkit.Diagnostics;
using Utopia.Core;

namespace Utopia.Server;

/// <summary>
/// 插件加载器接口，保证线程安全
/// </summary>
public interface IPluginLoader<PluginT>
{
    /// <summary>
    /// 已经实例化的插件
    /// </summary>
    IReadOnlyCollection<(Type, PluginT)> ActivePlugins { get; }

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

        public ActivePlguinEventArgs(Type pluginType, IContainer container) : base(true)
        {
            Guard.IsNotNull(pluginType);
            this.PluginType = pluginType;
            this.Container = container;
        }
    }

    /// <summary>
    /// 激活插件事件。<see cref="IPluginLoader{PluginT}.ActivePlguinEventArgs"/>。
    /// </summary>
    IEventManager<ActivePlguinEventArgs> ActivePlguinEvent { get; }

    /// <summary>
    /// 从dll文件中扫描并加载插件。将会注册所有实现了<see cref="PluginT"/>的类型。
    /// </summary>
    /// <param name="dllFile">dll文件</param>
    void Active(IContainer container, string dllFile);

    /// <summary>
    /// 激活指定类型的插件
    /// </summary>
    /// <param name="container">容器</param>
    /// <param name="type">插件类型，要求实现<see cref="PluginT"/>接口</param>
    void Active(IContainer container, Type type);
}
