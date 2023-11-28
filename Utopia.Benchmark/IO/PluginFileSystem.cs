// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utopia.Core.Plugin;

namespace Utopia.Core.Utilities.IO;

public class PluginFileSystem
    (string rootDirectory,string? packedFile,string? assemblyFile,string configurationDirectory) : IPluginFileSystem
{

    public string? PackedFile { get; init; } = packedFile is not null ? Path.GetFullPath(packedFile) : null;

    public string RootDirectory { get; init; } = Path.GetFullPath(rootDirectory + "/");

    public string? AssemblyFile { get; init; } = assemblyFile is not null ? Path.GetFullPath(assemblyFile) : null;

    public string ConfigurationDirectory => Path.GetFullPath(configurationDirectory);
}
