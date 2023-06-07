//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using Utopia.Core.Translate;

namespace Utopia.Core;

/// <summary>
/// 插件基础
/// </summary>
public interface IPluginBase
{
    /// <summary>
    /// 插件的人类可读名称
    /// </summary>
    TranslateKey Name { get; }

    /// <summary>
    /// 插件的描述
    /// </summary>
    TranslateKey Description { get; }

    /// <summary>
    /// 插件的许可协议
    /// </summary>
    string License { get; }

    /// <summary>
    /// 插件的版本号
    /// </summary>
    Version Version { get; }

    /// <summary>
    /// 用于标识插件的唯一ID
    /// </summary>
    Guuid Id { get; }

    /// <summary>
    /// 插件的网址，或者其他联系方式等。
    /// </summary>
    string Homepage { get; }

}
