using Utopia.Core.Collections;
using Utopia.Core.Exceptions;
using Utopia.Core.Utilities;
using Utopia.Server.Map;

namespace Utopia.Server.Entity;

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
    IEntity Create(Guuid guuid, byte[]? data);
}

public class EmptyEntityFactory : IEntityFactory
{
    public static readonly EmptyEntityFactory INSTANCE = new();

    public ISafeDictionary<Guuid, IEntity> Entities { get; }
        = new SafeDictionary<Guuid, IEntity>();

    public IEntity Create(Guuid guuid, byte[]? data)
    {
        if (this.Entities.TryGetValue(guuid, out var entity))
        {
            return entity!;
        }
        throw new EntityNotFoundException(guuid);
    }
}
