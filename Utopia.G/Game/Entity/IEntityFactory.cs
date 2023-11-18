// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

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

    public IGodotEntity Create(Guuid guuid, byte[]? data) => Entities.TryGetValue(guuid, out IGodotEntity? entity) ? entity! : throw new EntityNotFoundException(guuid);
}
