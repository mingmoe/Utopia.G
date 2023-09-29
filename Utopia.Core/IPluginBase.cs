//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
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
