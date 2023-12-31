// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nuke.Common.IO;

namespace Utopia.BuildScript;

public static class Utilities
{
    public static string GetProjectPath(this AbsolutePath path, string projectPath)
    {
        return path / $"Utopia.{projectPath}";
    }

    public static string GetProjectFilePath(this AbsolutePath path, string projectPath)
    {
        return path / $"Utopia.{projectPath}" / $"Utopia.{projectPath}.csproj";
    }
}
