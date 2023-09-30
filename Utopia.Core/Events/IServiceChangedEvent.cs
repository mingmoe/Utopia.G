#region copyright
// This file(may named IServiceChangedEvent.cs) is a part of the project: Utopia.Core.
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

namespace Utopia.Core.Events;

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
public interface IServiceChangedEvent<ServiceT> : IEvent
{
    public ServiceChangedType Type { get; }

    /// <summary>
    /// 要添加，删除，更新(新的那个)的对象
    /// </summary>
    public ServiceT Target { get; set; }

    /// <summary>
    /// 对于更新对象，存在一个旧对象
    /// </summary>
    public ServiceT? Old { get; }
}
