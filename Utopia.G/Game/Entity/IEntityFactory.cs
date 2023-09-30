#region copyright
// This file(may named IEntityFactory.cs) is a part of the project: Utopia.G.
// 
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// 
// This file is part of Utopia.G.
//
// Utopia.G is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Utopia.G is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with Utopia.G. If not, see <https://www.gnu.org/licenses/>.
#endregion

using Utopia.Core.Collections;
using Utopia.Core.Exceptions;
using Utopia.Core.Utilities;

namespace Utopia.G.Game.Entity;

/// <summary>
/// 实体工厂接口
/// </summary>
public interface IEntityFactory
{
    /// <summary>
    /// 生产一个实体
    /// </summary>
    /// <param name="guuid">想要创建的实体的ID</param>
    /// <param name="data">实体数据，通常是从存档中加载的或者是从服务端发送的.
    /// 如果服务端没发送数据则为数组 is null.
    /// 这个函数也可能被客户端调用,用于在客户端创建一些不需要服务端数据的实体.
    /// </param>
    /// <returns>实体</returns>
    IGodotEntity Create(Guuid guuid, byte[]? data);
}

public class EmptyEntityFactory : IEntityFactory
{
    public static readonly EmptyEntityFactory INSTANCE = new();

    public ISafeDictionary<Guuid, IGodotEntity> Entities { get; }
        = new SafeDictionary<Guuid, IGodotEntity>();

    public IGodotEntity Create(Guuid guuid, byte[]? data)
    {
        if (this.Entities.TryGetValue(guuid, out var entity))
        {
            return entity!;
        }
        throw new EntityNotFoundException(guuid);
    }
}
