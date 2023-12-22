// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using Utopia.Core.Translation;
using Utopia.Core.Utilities;

namespace Utopia.Core.Map;

public static class AreaInformation
{
    public const int XSize = 32;

    public const int YSize = 32;
}

/// <summary>
/// 气候
/// </summary>
public interface IPrecipitationInfo
{
    string Name { get; }

    Guuid ID { get; }
}

/// <summary>
/// 海拔
/// </summary>
public interface IElevationInfo
{
    string Name { get; }

    Guuid ID { get; }
}

/// <summary>
/// 气温
/// </summary>
public interface ITemperatureInfo
{
    string Name { get; }

    Guuid ID { get; }
}

/// <summary>
/// 生态环境
/// </summary>
public interface IBiomeInfo
{
    string Name { get; }
    Guuid ID { get; }
}

/// <summary>
/// 建筑
/// </summary>
public interface IConstructionInfo
{
    string Name { get; }
    Guuid ID { get; }
}
