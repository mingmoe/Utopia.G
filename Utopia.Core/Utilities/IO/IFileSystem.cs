#region copyright
// This file(may named IFileSystem.cs) is a part of the project: Utopia.Core.
// 
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// 
// This file is part of Utopia.Core.
//
// Utopia.Core is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Utopia.Core is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with Utopia.Core. If not, see <https://www.gnu.org/licenses/>.
#endregion

namespace Utopia.Core.Utilities.IO;

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
    string Configurations { get; }

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
        Directory.CreateDirectory(this.Configurations);
        Directory.CreateDirectory(this.Utilties);
        if (this.Server != null)
        {
            Directory.CreateDirectory(this.Server);
        }
    }

    string GetConfigurationOfPlugin(IPluginInformation plugin)
    {
        
        var path = Path.Join(this.Configurations, plugin.Id.ToString());
        return path;
    }
}
