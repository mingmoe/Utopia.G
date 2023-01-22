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

namespace Utopia.Server
{
    /// <summary>
    /// 插件加载器接口
    /// </summary>
    public interface IPluginLoader
    {
        /// <summary>
        /// 已经扫描到的插件
        /// </summary>
        IReadOnlyCollection<Type> LoadedPlugins { get; }

        /// <summary>
        /// 已经实例化的插件
        /// </summary>
        IReadOnlyCollection<(Type,IPlugin)> ActivePlugins { get; }

        /// <summary>
        /// 从dll文件中扫描并加载插件。将会注册所有实现了<see cref="IPlugin"/>的类型。
        /// </summary>
        /// <param name="dllFile">dll文件</param>
        void Load(string dllFile);

        /// <summary>
        /// 注册插件
        /// </summary>
        /// <param name="type">实现了IPlugin的类型</param>
        /// <exception cref="ArgumentException">如果类型无法赋值到IPlugin</exception>
        void Load(Type type);

        /// <summary>
        /// 激活插件
        /// </summary>
        /// <param name="container">容器</param>
        void Active(IContainer container);
    }
}
