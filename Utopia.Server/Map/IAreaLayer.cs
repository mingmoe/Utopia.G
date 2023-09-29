using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utopia.Core.Map;

namespace Utopia.Server.Map;

/// <summary>
/// 代表一个Z轴下的Area
/// </summary>
public interface IAreaLayer : Logic.IUpdatable
{
    bool TryGetBlock(FlatPosition position, out IBlock? block);

    /// <summary>
    /// Area坐标
    /// </summary>
    WorldPosition Position { get; }

    /// <summary>
    /// 标识目前区域所处的阶段
    /// </summary>
    GenerationStage Stage { get; set; }
}
