// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.IO;

namespace Utopia.G;

public class FileSystem : Core.IO.FileSystem
{
    public override string RootDirectory { get; } = Path.GetDirectoryName(Godot.OS.GetExecutablePath()) ?? ".";

    public override string? ServerDirectory => Path.Join(RootDirectory, "Server");

    private class ServerFileSystem(string root) : Core.IO.FileSystem
    {
        private readonly string _root = root;

        public override string RootDirectory => _root;

        public override string? ServerDirectory => null;
    }

    public Core.IO.FileSystem CreateServerFileSystem() => new ServerFileSystem(ServerDirectory!);
}

