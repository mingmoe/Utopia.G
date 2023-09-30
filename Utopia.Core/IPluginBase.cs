#region copyright
// This file(may named IPluginBase.cs) is a part of the project: Utopia.Core.
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

namespace Utopia.Core;

/// <summary>
/// 插件基础
/// </summary>
public interface IPluginBase
{
    /// <summary>
    /// 人类可读的名称
    /// </summary>
    ITranslatedString Name { get; }

    /// <summary>
    /// 人类可读的描述
    /// </summary>
    ITranslatedString Description { get; }

    /// <summary>
    /// 许可协议
    /// </summary>
    string License { get; }

    /// <summary>
    /// 版本号
    /// </summary>
    Version Version { get; }

    /// <summary>
    /// 唯一ID
    /// </summary>
    Guuid Id { get; }

    /// <summary>
    /// 网址，或者其他联系方式等。
    /// </summary>
    string Homepage { get; }

    /// <summary>
    /// 在这个函数中,而不是在构造函数中进行初始化
    /// </summary>
    void Active();
}
