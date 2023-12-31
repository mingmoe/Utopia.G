// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;

namespace Utopia.BuildScript;

interface INativeRelease : INukeBuild
{
    [Parameter] AbsolutePath ReleasePath { get; set; }

    Target ReleaseNativeLibraries => (_) =>
    {
        return _
        .Description("release native libraries")
        .Requires(() => ReleasePath)
        .DependsOn<INativeBuild>()
        .Executes(() =>
        {
            var path = RootDirectory / "precompiled-library" / "Release";

            foreach(var fs in path.GetFiles())
            {
                File.Copy(fs, ReleasePath / fs.Name);
            }
        });
    };
}

interface INativeBuild : INukeBuild
{
    [PathVariable("cmake")]
    Tool CMake { get; set; }

    Target CompileNativeLibraries => (_) =>
    {
        void Build(string projectName)
        {
            CMake("-S . -B build --fresh -Wno-dev -DCMAKE_BUILD_TYPE=Release",
                workingDirectory: RootDirectory.GetProjectPath(projectName),
                logOutput: false);
            CMake(" --build build --config Release",
                workingDirectory: RootDirectory.GetProjectPath(projectName),
                logOutput: false);
        }
        return _
        .Description("compile native(c/c++) libraries")
        .Requires(() => CMake)
        .Executes(() =>
        {
            Build("Kcp");
            Build("FastNoise");
        });
    };
}
