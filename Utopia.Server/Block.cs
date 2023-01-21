//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//

namespace Utopia.Core
{
    public class Block : IBlock
    {
        readonly List<IEntity> entities = new();
        readonly object locker = new();

        ulong cannotAccessable = 0;
        bool collidable = false;

        public bool Collidable
        {
            get
            {
                lock (locker)
                    return collidable;
            }
        }

        public bool Accessable
        {
            get
            {
                lock (locker)
                    return cannotAccessable == 0;
            }
        }

        public bool Contains(IEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));
            lock (locker)
            {
                return entities.Contains(entity);
            }
        }

        public long EntityCount()
        {
            lock (locker)
            {
                return this.entities.Count;
            }
        }

        public IReadOnlyCollection<IEntity> GetAllEntities()
        {
            lock (locker)
            {
                return entities;
            }
        }

        public bool IsEmpty()
        {
            lock (locker)
            {
                return this.entities.Count == 0;
            }
        }

        public void RemoveEntity(IEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));
            lock (locker)
            {
                var found = entities.Remove(entity);

                if (!found)
                {
                    return;
                }
                // update
                if (entity.Collidable)
                {
                    this.collidable = false;
                }
                if (entity.Accessible)
                {
                    return;
                }

                cannotAccessable--;
            }
        }

        public bool TryAddEntity(IEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));
            lock (locker)
            {
                if (collidable && entity.Collidable)
                {
                    return false;
                }
                entities.Add(entity);

                this.collidable = entity.Collidable;

                if (!entity.Accessible)
                {
                    cannotAccessable++;
                }
            }
            return true;
        }

    }
}
