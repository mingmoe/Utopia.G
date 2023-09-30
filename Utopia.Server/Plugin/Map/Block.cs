#region copyright
// This file(may named Block.cs) is a part of the project: Utopia.Server.
// 
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// 
// This file is part of Utopia.Server.
//
// Utopia.Server is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Utopia.Server is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with Utopia.Server. If not, see <https://www.gnu.org/licenses/>.
#endregion

using System.Net;
using Utopia.Core.Map;
using Utopia.Core.Utilities.IO;
using Utopia.Server.Map;

namespace Utopia.Server.Plugin.Map;

public class Block : IBlock
{
    readonly List<IEntity> _entities = new();
    readonly object _locker = new();

    ulong _cannotAccessable = 0;
    bool _collidable = false;

    public bool Collidable
    {
        get
        {
            lock (this._locker)
            {
                return this._collidable;
            }
        }
    }

    public bool Accessable
    {
        get
        {
            lock (this._locker)
            {
                return this._cannotAccessable == 0;
            }
        }
    }

    public Block(WorldPosition position)
    {
        this.Position = position;
    }

    public WorldPosition Position { get; init; }

    public bool Contains(IEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));
        lock (this._locker)
        {
            return this._entities.Contains(entity);
        }
    }

    public long EntityCount()
    {
        lock (this._locker)
        {
            return this._entities.Count;
        }
    }

    public IReadOnlyCollection<IEntity> GetAllEntities()
    {
        lock (this._locker)
        {
            // to array to prevent user to change list
            return this._entities.ToArray();
        }
    }

    public bool IsEmpty()
    {
        lock (this._locker)
        {
            return this._entities.Count == 0;
        }
    }

    public void RemoveEntity(IEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));
        lock (this._locker)
        {
            var found = this._entities.Remove(entity);

            if (!found)
            {
                return;
            }
            // update
            if (entity.Collidable)
            {
                this._collidable = false;
            }
            if (entity.Accessible)
            {
                return;
            }

            this._cannotAccessable--;
        }
    }

    public bool TryAddEntity(IEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));
        lock (this._locker)
        {
            if (this._collidable && entity.Collidable)
            {
                return false;
            }
            this._entities.Add(entity);
            entity.WorldPosition = this.Position;

            this._collidable = entity.Collidable;

            if (!entity.Accessible)
            {
                this._cannotAccessable++;
            }
        }
        return true;
    }

    public void LogicUpdate()
    {
        lock (this._locker)
        {
            foreach (var item in this._entities)
            {
                item.LogicUpdate();
            }
        }
    }

    public byte[] Save()
    {
        MemoryStream stream = new();
        lock (this._locker)
        {
            StreamUtility.WriteInt(stream, this._entities.Count).Wait();
            foreach (var item in this._entities)
            {
                StreamUtility.WriteStringWithLength(stream, item.Id.ToString()).Wait();
                StreamUtility.WriteDataWithLength(stream, item.Save()).Wait();
            }
        }

        return stream.ToArray();
    }
}
