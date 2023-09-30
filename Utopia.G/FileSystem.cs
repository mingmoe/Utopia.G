#region copyright
// This file(may named FileSystem.cs) is a part of the project: Utopia.G.
// 
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// 
// This file is part of Utopia.G.
//
// Utopia.G is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Utopia.G is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with Utopia.G. If not, see <https://www.gnu.org/licenses/>.
#endregion

using System.IO;

namespace Utopia.G;

public class FileSystem : Core.Utilities.IO.FileSystem
{
    public override string Root { get; } = Path.GetDirectoryName(Godot.OS.GetExecutablePath()) ?? ".";

    public override string? Server => Path.Join(this.Root, "Server");

    private class ServerFileSystem : Core.Utilities.IO.FileSystem
    {
        private readonly string _root;

        public override string Root => this._root;

        public override string? Server => null;

        public ServerFileSystem(string root)
        {
            this._root = root;
        }
    }

    public Core.Utilities.IO.FileSystem CreateServerFileSystem()
    {
        return new ServerFileSystem(this.Server!);
    }
}

