#region copyright
// This file(may named EntityManager.cs) is a part of the project: Utopia.G.
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
/// 实体管理器
/// </summary>
public interface IEntityManager : ISafeDictionary<Guuid, IEntityFactory>
{
    public IGodotEntity Create(Guuid id, byte[] data)
    {
        if (this.TryGetValue(id, out var factory))
        {
            return factory!.Create(id, data);
        }
        throw new EntityNotFoundException(id);
    }
}

public class EntityManager : SafeDictionary<Guuid, IEntityFactory>, IEntityManager
{
}
