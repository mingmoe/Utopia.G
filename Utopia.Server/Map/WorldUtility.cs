#region copyright
// This file(may named WorldUtility.cs) is a part of the project: Utopia.Server.
// 
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// 
// This file is part of Utopia.Server.
//
// Utopia.Server is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Utopia.Server is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with Utopia.Server. If not, see <https://www.gnu.org/licenses/>.
#endregion

using Utopia.Core.Collections;
using Utopia.Core.Map;

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
