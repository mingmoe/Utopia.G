//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Utopia.Core;

namespace Utopia.G
{
    /// <summary>
    /// 客户端的实体管理器。
    /// </summary>
    public class EntityManager
    {

        readonly ConcurrentDictionary<Guuid, IEntityFactory> factories = new();

        /// <summary>
        /// 尝试注册实体
        /// </summary>
        /// <param name="entity">要注册的实体id</param>
        /// <param name="factory">生产此实体的工厂</param>
        /// <returns>如果此实体ID已经被占用，返回false。否则注册成功，返回true。</returns>
        public bool TryRegisterEntity(Guuid entityId, IEntityFactory factory)
        {
            return factories.TryAdd(entityId, factory);
        }

        /// <summary>
        /// 删除实体注册
        /// </summary>
        public void Unregister(Guuid entityId)
        {
            factories.TryRemove(entityId, out _);
        }

        /// <summary>
        /// 检查实体工厂是否被注册
        /// </summary>
        public bool Contains(Guuid entityId)
        {
            return factories.ContainsKey(entityId);
        }

        /// <summary>
        /// 获取所有已经注册的实体
        /// </summary>
        /// <returns>已经注册的实体</returns>
        public IReadOnlyCollection<KeyValuePair<Guuid, IEntityFactory>> GetAllRegisteredEntity()
        {
            return factories.ToList();
        }

        /// <summary>
        /// 创造实体
        /// </summary>
        /// <param name="entityId">实体ID</param>
        /// <param name="data">实体数据</param>
        /// <param name="entity">返回的实体，如果实体ID尚未注册，则返回null</param>
        /// <returns>如果实体ID为注册，返回false。否则返回true，表示创造成功。</returns>
        public bool TryCreate(Guuid entityId, byte[]? data, out IEntity? entity)
        {
            if (factories.TryGetValue(entityId, out IEntityFactory? factory))
            {
                entity = factory.Create(data);
                return true;
            }
            entity = null;
            return false;
        }

    }
}
