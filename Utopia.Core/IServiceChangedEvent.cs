//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//

namespace Utopia.Core;

/// <summary>
/// 服务变动类型
/// </summary>
public enum ServiceChangedType
{
    /// <summary>
    /// 添加服务
    /// </summary>
    Add,
    /// <summary>
    /// 更新服务
    /// </summary>
    Update,
    /// <summary>
    /// 删除服务
    /// </summary>
    Delete
}

/// <summary>
/// 服务更新事件。
/// 事件的参数为触发事件的对象（即新添加的对象，被删除的对象，对于更新对象，将会传递新对象）
/// 事件的结果将被忽略，并且初始化为null。
/// 事件可以被取消。
/// </summary>
public interface IServiceChangedEvent<ServiceT> : IEvent<ServiceChangedType, ServiceT>
{
}
