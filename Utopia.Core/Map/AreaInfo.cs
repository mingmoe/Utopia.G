using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utopia.Core.Translate;
using Utopia.Core.Utilities;

namespace Utopia.Core.Map;

/// <summary>
/// 气候
/// </summary>
public interface IPrecipitationInfo
{
    ITranslatedString Name { get; }
    Guuid ID { get; }
}

/// <summary>
/// 海拔
/// </summary>
public interface IElevationInfo
{
    ITranslatedString Name { get; }
    Guuid ID { get; }
}

/// <summary>
/// 气温
/// </summary>
public interface ITemperatureInfo
{
    ITranslatedString Name { get; }

    Guuid ID { get; }
}

/// <summary>
/// 生态环境
/// </summary>
public interface IBiomeInfo
{
    ITranslatedString Name { get; }
    Guuid ID { get; }
}

/// <summary>
/// 建筑
/// </summary>
public interface IConstructionInfo
{
    ITranslatedString Name { get; }
    Guuid ID { get; }
}
