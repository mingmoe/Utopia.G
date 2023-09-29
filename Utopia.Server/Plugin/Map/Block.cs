//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//

using Utopia.Core.Map;
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
            lock (_locker)
            {
                return _collidable;
            }
        }
    }

    public bool Accessable
    {
        get
        {
            lock (_locker)
            {
                return _cannotAccessable == 0;
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
        lock (_locker)
        {
            return _entities.Contains(entity);
        }
    }

    public long EntityCount()
    {
        lock (_locker)
        {
            return this._entities.Count;
        }
    }

    public IReadOnlyCollection<IEntity> GetAllEntities()
    {
        lock (_locker)
        {
            // to array to prevent user to change list
            return _entities.ToArray();
        }
    }

    public bool IsEmpty()
    {
        lock (_locker)
        {
            return this._entities.Count == 0;
        }
    }

    public void RemoveEntity(IEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));
        lock (_locker)
        {
            var found = _entities.Remove(entity);

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

            _cannotAccessable--;
        }
    }

    public bool TryAddEntity(IEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));
        lock (_locker)
        {
            if (_collidable && entity.Collidable)
            {
                return false;
            }
            _entities.Add(entity);
            entity.WorldPosition = this.Position;

            this._collidable = entity.Collidable;

            if (!entity.Accessible)
            {
                _cannotAccessable++;
            }
        }
        return true;
    }

    public void LogicUpdate()
    {
        lock (_locker)
        {
            foreach (var item in _entities)
            {
                item.LogicUpdate();
            }
        }
    }
}
