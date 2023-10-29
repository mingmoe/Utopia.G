// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Framework;

namespace Utopia.Build
{
    public class GeneratePluginSourceTask : Microsoft.Build.Utilities.Task
    {

        public bool AccessVersionFromParent { get; set; } = false;

        public bool AccessPluginFromParent { get; set; } = false;

        [Required]
        public string ProjectDir { get; set; }

        [Required]
        public string Namespace { get; set; }

        [Required]
        public string Generate { get; set; }

        [Output]
        public string[] Arguments { get; set; }

        public override bool Execute()
        {
            string projectDir = ProjectDir;
            string targetNamespace = Namespace;

            string version = AccessVersionFromParent ? Path.Combine(projectDir, "../version.txt") :
                Path.Combine(projectDir, "version.txt");

            string info = AccessPluginFromParent ? Path.Combine(projectDir, "../utopia.toml") :
                Path.Combine(projectDir, "utopia.toml");

            var arguments = new List<string>
            {
                "generate",
                "--version-file",
                version.ToString(),
                "--plugin-info-file",
                info.ToString(),
                "--project",
                projectDir,
                "--namespace",
                targetNamespace,
                Generate
            };

            Arguments = arguments.ToArray();

            return true;
        }
    }
}
