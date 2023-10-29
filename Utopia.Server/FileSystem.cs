// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

namespace Utopia.Server;

public class FileSystem : Core.Utilities.IO.FileSystem
{
    public override string RootDirectory => Path.GetPathRoot(System.Reflection.Assembly.GetExecutingAssembly().Location ?? ".") ?? ".";

    public override string? ServerDirectory => null;
}
