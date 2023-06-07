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
/// 翻译条目结果，非线程安全。
/// </summary>
/// <param name="Cached">翻译缓存</param>
/// <param name="Id">翻译缓存的ID，这个数值可以用于热更新翻译</param>
public record class TranslateResult(string? Cached, long Id);

/// <summary>
/// 翻译键，非线程安全。
/// </summary>
/// <param name="TranslateProviderId">翻译提供者ID</param>
/// <param name="TranslateItemId">翻译条目ID</param>
/// <param name="Id">翻译缓存</param>
/// <param name="Comment">翻译的注释，一般没啥用，给翻译人员使用</param>
public record struct TranslateKey(in string TranslateItemId, in string Comment,
    in string? TranslateProviderId = null, TranslateResult? Id = null)
{
    public static TranslateKey Create(in string itemId, in string comment, in string? providerId = null)
    {
        Guard.IsNotNull(itemId);
        Guard.IsNotNull(comment);

        var n = (TranslateKey)Activator.CreateInstance(typeof(TranslateKey))!;

        n.TranslateItemId = itemId;
        n.Comment = comment;
        n.TranslateProviderId = providerId;

        return n;
    }

}
