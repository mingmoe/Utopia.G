//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using System.IO;

namespace Utopia.G;

public class FileSystem : Utopia.Core.FileSystem
{
    public override string Root { get; } = Path.GetDirectoryName(Godot.OS.GetExecutablePath()) ?? ".";

    public override string? Server => Path.Join(this.Root, "Server");

    private class ServerFileSystem : Utopia.Core.FileSystem
    {
        private readonly string _root;

        public override string Root => this._root;

        public override string? Server => null;

        public ServerFileSystem(string root)
        {
            this._root = root;
        }
    }

    public Utopia.Core.FileSystem CreateServerFileSystem()
    {
        return new ServerFileSystem(this.Server!);
    }
}

