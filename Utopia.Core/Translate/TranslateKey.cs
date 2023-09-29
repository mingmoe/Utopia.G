//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//

using CommunityToolkit.Diagnostics;
using Jeffijoe.MessageFormat;

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
