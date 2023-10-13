using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Utopia.Build
{
    /// <summary>
    /// This task will run Utopia.Tools.
    /// </summary>
    public class RunUtopiaToolTask : Microsoft.Build.Utilities.Task
    {

        /// <summary>
        /// If is null or <see cref="string.Empty"/>,
        /// use `dotnet tool` to run Utopia.Tools.
        /// </summary>
        public string RunFromProject { get; set; } = null;

        public string ProjectCondifuration { get; set; } = "Release";

        /// <summary>
        /// This flag mark the position of Utopia.Tools.
        /// If <see cref="UseFromProject"/> is empty, this will be ignored.
        /// </summary>
        public bool LocalTool { get; set; } = false;

        public string DotnetPath { get; set; } = "dotnet";

        [Required]
        public string[] Arguments { get; set; } = Array.Empty<string>();

        public override bool Execute()
        {
            var sb = new StringBuilder();
            var proc = new Process();
            var info = proc.StartInfo;
            info.RedirectStandardError = true;
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;

            // find tools
            if (!string.IsNullOrEmpty(this.RunFromProject))
            {
                info.FileName = this.DotnetPath;
                sb.Append($" run  --configuration {this.ProjectCondifuration} --project {this.RunFromProject} -- ");
            }
            else
            {
                if (this.LocalTool)
                {
                    info.FileName = "Utopia.Tools";

                }
                else
                {
                    info.FileName = this.DotnetPath;
                    sb.Append(" tool run Utopia.Tools ");
                }
            }

            // build argument
            foreach(var argument in this.Arguments)
            {
                var formatted = argument.Replace("\"","\\\"");
                formatted = formatted.Replace("'", "\\'");
                sb.Append($" \"{formatted}\" ");
            }
            info.Arguments = sb.ToString();

            // run
            proc.Start();
            proc.WaitForExit();

            var code = proc.ExitCode;

            this.Log.LogMessageFromText($"Utopia.Tools: run [{info.FileName} {info.Arguments}] exit with code {code}", MessageImportance.High);

            this.Log.LogError("stdio:{0}",proc.StandardOutput.ReadToEnd());
            this.Log.LogError("stderr:{0}", proc.StandardError.ReadToEnd());

            return code == 0;
        }
    }
}
