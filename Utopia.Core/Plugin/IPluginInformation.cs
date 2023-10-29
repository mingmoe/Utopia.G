// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using Utopia.Core.Transition;
using Utopia.Core.Utilities;

namespace Utopia.Core.Plugin;

/// <summary>
/// 插件基础
/// </summary>
public interface IPluginInformation
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
}

