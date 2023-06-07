using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utopia.Core;

namespace Utopia.Server.Map;
public static class WorldUtility
{

    /// <summary>
    /// 获取坐标，自动查询世界
    /// </summary>
    /// <param name=""></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public static bool TryGetBlock(this Utopia.Core.IServiceProvider provider, WorldPosition position, out Map.IBlock? block)
    {
        block = null;
        if (!provider.TryGetService<SafeDictionary<long, Map.IWorld>>(out var world))
        {
            return false;
        }

        if (!world!.TryGetValue(position.Id, out Map.IWorld? w))
        {
            return false;
        }

        return w!.TryGetBlock(position.ToPos(), out block);
    }

}
