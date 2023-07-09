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
    /// PostgreSql数据库在<see cref="Utilities"/>下的目录。
    /// </summary>
    public const string PgSqlPath = "pgsql";

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
    /// 游戏的工具目录。用于存放一些其他东西。
    /// </summary>
    string Utilties { get; }

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
        Directory.CreateDirectory(this.Utilties);
        if (this.Server != null)
        {
            Directory.CreateDirectory(this.Server);
        }
    }

    /// <summary>
    /// 尝试获取内置postgresql数据库的安装路径。
    /// 如果为null。则说明数据库未安装。
    /// </summary>
    /// <returns>pg安装的根目录和pg_ctl实用程序</returns>
    (string pgDir, string pgExecutable)? TryGetPostgreSqlPath()
    {
        var dir = Path.Join(this.Utilties, PgSqlPath);

        if (!Directory.Exists(dir))
        {
            return null;
        }
        // check executable file exists
        string file;

        if (OperatingSystem.IsWindows())
        {
            file = "bin/pg_ctl.exe";
        }
        else
        {
            file = "bin/pg_ctl";
        }

        file = Path.Join(dir, file);

        if (!File.Exists(file))
        {
            return null;
        }

        return new(dir, file);
    }
}
