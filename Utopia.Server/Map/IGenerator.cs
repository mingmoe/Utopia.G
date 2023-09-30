#region copyright
// This file(may named IGenerator.cs) is a part of the project: Utopia.Server.
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

using Utopia.Core.Map;
using Utopia.Core.Translate;
using Utopia.Core.Utilities;

namespace Utopia.Server.Map;

/// <summary>
/// 标识生成世界的阶段
/// </summary>
public enum GenerationStage
{
    NoStart,
    Before,
    Climate,
    Biome,
    Construction,
    Feature,
    Bio,
    Post,
    Finish,
}

/// <summary>
/// 气候
/// </summary>
public interface IPrecipitation : IPrecipitationInfo
{
}

/// <summary>
/// 海拔
/// </summary>
public interface IElevation : IElevationInfo
{
}

public interface ITemperature : ITemperatureInfo
{
}

/// <summary>
/// 生态群落
/// </summary>
public interface IBiome : IBiomeInfo
{
}

public interface IConstruction : IConstructionInfo
{
}

public class EmptyWorldInfo
{
#pragma warning disable CA1822 // Mark members as static
    public ITranslatedString Name => new TranslatedString("Empty World Info for DEBUG");

    public Guuid ID => Guuid.NewUtopiaGuuid("world", "empty");
#pragma warning restore CA1822 // Mark members as static
}

public class EmptyPrecipiation : EmptyWorldInfo, IPrecipitation
{
}
public class EmptyElevation : EmptyWorldInfo, IElevation
{
}
public class EmptyBiome : EmptyWorldInfo, IBiome
{
}
public class EmptyTemperature : EmptyWorldInfo, ITemperature
{
}
public class EmptyConstruction : EmptyWorldInfo, IConstruction
{
}

/// <summary>
/// 生态系统生成器生成器
/// </summary>
public interface IClimateGenerator
{
    /// <summary>
    /// 获取降水量
    /// </summary>
    /// <param name="position">要获取高度的位置.单位为Area</param>
    IPrecipitation GetPrecipitation(Position position);

    /// <summary>
    /// 获取海拔高度
    /// </summary>
    /// <param name="position">要获取高度的位置.单位为Area</param>
    IElevation GetElevation(Position position);

    /// <summary>
    /// 获取气温
    /// </summary>
    /// <param name="position">要获取气温的位置.单位为Area</param>
    ITemperature GetTemperature(Position position);
}

/// <summary>
/// 生态群落生成器.主要用于生成自然环境
/// </summary>
public interface IBiomeGenerator
{
    /// <summary>
    /// 获取生态系统
    /// </summary>
    /// <param name="position">单位为Area</param>
    IBiome GetBiome(FlatPositionWithId position);
}

/// <summary>
/// 结构生成器.
/// 这些结构会在地图上被表明,有自己的名字.
/// </summary>
public interface IConstructionGenerator
{
    /// <summary>
    /// 获取建筑类型.
    /// </summary>
    /// <param name="position">单位为Area</param>
    IConstruction GetConstruction(FlatPositionWithId position);
}

/// <summary>
/// 地形特征.(一般是小型的东西,且不分人造和自然物,例如树木,水井之类的).
/// 这些东西不会在地图上被标注,也没有名字.
/// </summary>
public interface IBiomeFeatureGenerator
{
    void Generate(IAreaLayer areaLayer);
}

/// <summary>
/// 世界生成器
/// </summary>
public interface IWorldGenerator
{
    /// <summary>
    /// 生成区域
    /// </summary>
    /// <param name="area">要生成的区域的位置.单位为Area.</param>
    void Generate(IAreaLayer area);
}
