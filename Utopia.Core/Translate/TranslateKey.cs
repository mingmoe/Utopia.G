#region copyright
// This file(may named TranslateKey.cs) is a part of the project: Utopia.Core.
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

namespace Utopia.Core.Translate;

/// <summary>
/// 翻译键，线程安全。
/// </summary>
/// <param name="TranslateProviderId">翻译提供者ID</param>
/// <param name="TranslateItemId">翻译条目ID</param>
/// <param name="Id">翻译缓存</param>
/// <param name="Comment">翻译的注释，一般没啥用，给翻译人员使用</param>
public readonly record struct TranslateKey(in string TranslateItemId, in string Comment,
    in string? TranslateProviderId = null)
{
    /// <summary>
    /// 使用这个函数来防止源代码检测器检测
    /// </summary>
    public static TranslateKey Create(in string itemId, in string comment, in string? providerId = null)
    {
        ArgumentNullException.ThrowIfNull(itemId, nameof(itemId));
        ArgumentNullException.ThrowIfNull(comment, nameof(comment));

        var n = (TranslateKey)Activator.CreateInstance(typeof(TranslateKey),
            itemId,
            comment,
            providerId)!;

        return n;
    }
}
