// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using Utopia.Core.Map;
using Utopia.Core.Utilities.IO;

namespace Utopia.Server.Map;

public struct Block : IBlock
{
    private readonly List<IEntity> _entities = new();
    private ulong _cannotAccessible = 0;
    private bool _hasCollision = false;
    private readonly ReaderWriterLockSlim _readerWriterLock = new();

    public bool HasCollision
    {
        get
        {
            return _hasCollision;
        }
    }

    public bool Accessible
    {
        get
        {
            return _cannotAccessible == 0;
        }
    }

    public Block(WorldPosition position) => Position = position;

    public WorldPosition Position { get; init; }

    public ReaderWriterLockSlim @lock => _readerWriterLock;

    public bool Contains(IEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));
        return _entities.Contains(entity);
    }

    public long EntityCount()
    {
        return _entities.Count;
    }

    public IReadOnlyCollection<IEntity> GetAllEntities()
    {
        // to array to prevent user to change list
        return _entities.ToArray();
    }

    public bool IsEmpty()
    {
        return _entities.Count == 0;
    }

    public void RemoveEntity(IEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        bool found = _entities.Remove(entity);

        if (!found)
        {
            return;
        }
        // update
        if (entity.Collidable)
        {
            _hasCollision = false;
        }
        if (entity.Accessible)
        {
            return;
        }

        _cannotAccessible--;
    }

    public bool TryAddEntity(IEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));
        if (_hasCollision && entity.Collidable)
        {
            return false;
        }
        _entities.Add(entity);
        entity.WorldPosition = Position;

        _hasCollision = entity.Collidable;

        if (!entity.Accessible)
        {
            _cannotAccessible++;
        }
        return true;
    }

    public void LogicUpdate()
    {
        // TODO: LOCK
        foreach (IEntity item in _entities)
        {
            item.LogicUpdate();
        }
    }

    public byte[] Save()
    {
        MemoryStream stream = new();
        StreamUtility.WriteInt(stream, _entities.Count).Wait();
        foreach (IEntity item in _entities)
        {
            StreamUtility.WriteStringWithLength(stream, item.Id.ToString()).Wait();
            StreamUtility.WriteDataWithLength(stream, item.Save()).Wait();
        }

        return stream.ToArray();
    }
}
