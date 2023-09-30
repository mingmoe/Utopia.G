#region copyright
// This file(may named FileSystem.cs) is a part of the project: Utopia.Core.
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
/// 文件系统
/// </summary>
public abstract class FileSystem : IFileSystem
{
    public abstract string Root { get; }

    public string Asserts => Path.Join(this.Root, "Asserts");

    public string Worlds => Path.Join(this.Root, "Worlds");

    public string Characters => Path.Join(this.Root, "Characters");

    public string Plugins => Path.Join(this.Root, "Plugins");

    public string Configuraions => Path.Join(this.Root, "Configurations");

    public string Utilties => Path.Join(this.Root, "Utilties");

    public abstract string? Server { get; }
}
