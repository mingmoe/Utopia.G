using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using System.Text;

namespace Utopia.MSBuild
{
    public class GeneratePluginInformationTask : Microsoft.Build.Utilities.Task
    {

        public bool AccessVersionFromParent { get; set; } = false;

        public bool AccessPluginFromParent { get; set; } = false;

        [Required]
        public string ProjectDir { get;set; }

        [Required]
        public string Namespace { get;set; }

        [Output]
        public string[] Arguments { get; set; }

        public override bool Execute()
        {
            var projectDir = this.ProjectDir;
            var targetNamespace = this.Namespace;

            var version = this.AccessVersionFromParent ? Path.Combine(projectDir, "../version.txt") :
                Path.Combine(projectDir,"version.txt");

            var info = this.AccessPluginFromParent ? Path.Combine(projectDir, "../utopia.toml") :
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
                "pluginInfo"
            };

            this.Arguments = arguments.ToArray();

            return true;
        }
    }
}
