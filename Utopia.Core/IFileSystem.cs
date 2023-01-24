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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Utopia.Core;

/// <summary>
/// 文件系统接口
/// </summary>
public interface IFileSystem
{
    /// <summary>
    /// 游戏的根目录
    /// </summary>
    string Root { get; }

    /// <summary>
    /// 游戏的资产目录
    /// </summary>
    string Asserts { get; }

    /// <summary>
    /// 游戏的世界目录
    /// </summary>
    string Worlds { get; }

    /// <summary>
    /// 游戏的角色目录
    /// </summary>
    string Characters { get; }

    /// <summary>
    /// 游戏的插件目录
    /// </summary>
    string Plugins { get; }

    /// <summary>
    /// 游戏的配置文件目录
    /// </summary>
    string Configuraions { get; }
}
