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

    public override string? Server => "Server";
}

