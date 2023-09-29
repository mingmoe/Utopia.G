//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//

namespace Utopia.Server;

public class FileSystem : Core.Utilities.IO.FileSystem
{
    public override string Root => Path.GetPathRoot(System.Reflection.Assembly.GetExecutingAssembly().Location ?? ".") ?? ".";

    public override string? Server => null;
}
