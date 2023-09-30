#region copyright
// This file(may named ITranslateProvider.cs) is a part of the project: Utopia.Core.
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

using Utopia.Core.Utilities;

namespace Utopia.Core.Translate;

/// <summary>
/// 负责提供翻译的类.应该确保线程安全.
/// </summary>
public interface ITranslateProvider
{
    /// <summary>
    /// 获取翻译条目
    /// </summary>
    /// <param name="language">目标语言</param>
    /// <param name="id">翻译条目Id</param>
    /// <param name="result">结果，如果条目不存在，返回null</param>
    /// <returns>如果获取成功，找到翻译条目，返回true。</returns>
    bool TryGetItem(TranslateIdentifence language, Guuid id, out string? result);

    /// <summary>
    /// 查询编译条目是否存在
    /// </summary>
    /// <param name="language">目标语言</param>
    /// <param name="id">翻译条目id</param>
    /// <returns>如果翻译条目存在，返回true，否则返回false</returns>
    bool Contain(TranslateIdentifence language, Guuid id);
}
