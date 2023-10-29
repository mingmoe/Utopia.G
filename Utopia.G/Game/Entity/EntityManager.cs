// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using Utopia.Core.Collections;
using Utopia.Core.Exceptions;
using Utopia.Core.Utilities;

namespace Utopia.G.Game.Entity;

/// <summary>
/// 实体管理器
/// </summary>
public interface IEntityManager : ISafeDictionary<Guuid, IEntityFactory>
{
    public IGodotEntity Create(Guuid id, byte[] data) => TryGetValue(id, out IEntityFactory? factory) ? factory!.Create(id, data) : throw new EntityNotFoundException(id);
}

public class EntityManager : SafeDictionary<Guuid, IEntityFactory>, IEntityManager
{
}
