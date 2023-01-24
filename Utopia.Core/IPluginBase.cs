//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

}
