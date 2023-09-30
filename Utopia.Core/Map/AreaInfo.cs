#region copyright
// This file(may named AreaInfo.cs) is a part of the project: Utopia.Core.
// 
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// 
// This file is part of Utopia.Core.
//
// Utopia.Core is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Utopia.Core is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with Utopia.Core. If not, see <https://www.gnu.org/licenses/>.
#endregion

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
