//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//

namespace Utopia.Core.Translate;

/// <summary>
/// 负责提供翻译的类
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
    bool Contain(TranslateIdentifence language,Guuid id);
}
