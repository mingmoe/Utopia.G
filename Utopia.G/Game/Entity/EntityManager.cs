using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
