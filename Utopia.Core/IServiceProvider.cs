//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utopia.Core;

/// <summary>
/// 服务提供者接口
/// </summary>
public interface IServiceProvider
{
    /// <summary>
    /// 获取服务
    /// </summary>
    /// <typeparam name="T">服务类型</typeparam>
    /// <param name="service">获取到的服务，如果服务无效，则返回null</param>
    /// <returns>如果获取成功，返回true</returns>
    bool TryGetService<T>(out T? service);

    /// <summary>
    /// 获取服务
    /// </summary>
    /// <typeparam name="T"><服务类型/typeparam>
    /// <exception cref="InvalidOperationException">如果相应的类型尚未注册，则引发此异常</exception>
    /// <returns>返回值</returns>
    T GetService<T>();

    /// <summary>
    /// 检查服务是否存在
    /// </summary>
    /// <typeparam name="T">要检查的服务类型</typeparam>
    /// <returns>如果服务器存在则返回true</returns>
    bool HasService<T>();

    /// <summary>
    /// 尝试注册服务
    /// </summary>
    /// <typeparam name="T">服务类型</typeparam>
    /// <param name="service">尝试注册的服务</param>
    /// <returns>如果注册成功，返回true。如果该类型已经存在服务，则返回false。</returns>
    bool TryRegisterService<T>(T service);

    /// <summary>
    /// 删除服务
    /// </summary>
    /// <typeparam name="T">要删除的服务类型</typeparam>
    void RemoveService<T>();

    /// <summary>
    /// 获取所有已经注册的服务
    /// </summary>
    /// <returns></returns>
    IReadOnlyCollection<object> GetServices();
}
