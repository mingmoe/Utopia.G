//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using Autofac;

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
    void Active(IContainer container,Type type);
}
