//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//

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

    /// <summary>
    /// 对于游戏的客户端，这是服务器的Root文件夹。
    /// 对于服务器，返回null。
    /// </summary>
    string? Server { get; }

    /// <summary>
    /// 对于不存在的目录，则创建一个
    /// </summary>
    void CreateIfNotExist()
    {
        Directory.CreateDirectory(this.Root);
        Directory.CreateDirectory(this.Asserts);
        Directory.CreateDirectory(this.Worlds);
        Directory.CreateDirectory(this.Characters);
        Directory.CreateDirectory(this.Plugins);
        Directory.CreateDirectory(this.Configuraions);
        if (this.Server != null)
        {
            Directory.CreateDirectory(this.Server);
        }
    }
}
