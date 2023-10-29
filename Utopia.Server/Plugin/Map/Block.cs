// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using Utopia.Core.Map;
using Utopia.Core.Utilities.IO;
using Utopia.Server.Map;

namespace Utopia.Server.Plugin.Map;

public class Block : IBlock
{
    private readonly List<IEntity> _entities = new();
    private readonly object _locker = new();
    private ulong _cannotAccessable = 0;
    private bool _collidable = false;

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

    public Block(WorldPosition position) => Position = position;

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
            return _entities.Count;
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
            return _entities.Count == 0;
        }
    }

    public void RemoveEntity(IEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));
        lock (_locker)
        {
            bool found = _entities.Remove(entity);

            if (!found)
            {
                return;
            }
            // update
            if (entity.Collidable)
            {
                _collidable = false;
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
            entity.WorldPosition = Position;

            _collidable = entity.Collidable;

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
            foreach (IEntity item in _entities)
            {
                item.LogicUpdate();
            }
        }
    }

    public byte[] Save()
    {
        MemoryStream stream = new();
        lock (_locker)
        {
            StreamUtility.WriteInt(stream, _entities.Count).Wait();
            foreach (IEntity item in _entities)
            {
                StreamUtility.WriteStringWithLength(stream, item.Id.ToString()).Wait();
                StreamUtility.WriteDataWithLength(stream, item.Save()).Wait();
            }
        }

        return stream.ToArray();
    }

    public void OperateEntities(Action<IList<IEntity>> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        lock (_locker)
        {
            action.Invoke(_entities);
        }
    }
}
