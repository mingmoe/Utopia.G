using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

        public string PwshPath { get; set; } = "pwsh";

        [Required]
        public string[] Arguments { get; set; } = Array.Empty<string>();

        private static string _EscapePwsh(string str)
        {
            var formatted = str.Replace("\"", "`\"");
            formatted = formatted.Replace("'", "`\\'");
            return formatted;
        }

        public override bool Execute()
        {
            var sb = new StringBuilder();

            // find tools
            string tool;

            if (!string.IsNullOrEmpty(this.RunFromProject))
            {
                tool = _EscapePwsh(this.DotnetPath);
                sb.Append($" run  --configuration \"{_EscapePwsh(this.ProjectCondifuration)}\" --project \"{_EscapePwsh(this.RunFromProject)}\" -- ");
            }
            else
            {
                if (!this.LocalTool)
                {
                    tool = _EscapePwsh("Utopia.Tools");

                }
                else
                {
                    tool = _EscapePwsh(this.DotnetPath);
                    sb.Append(" tool run Utopia.Tools ");
                }
            }

            // build argument
            foreach(var argument in this.Arguments)
            {
                sb.Append($" \"{_EscapePwsh(argument)}\" ");
            }

            // build script file
            var scriptFile = Path.GetTempFileName() + ".ps1";

            while(File.Exists(scriptFile))
            {
                scriptFile = Path.GetTempFileName() + ".ps1";
            }
            var script = $"\n&\"{tool}\" {sb}\n";

            this.Log.LogMessage(MessageImportance.High,$"generate powershell script file({scriptFile}):{script}");

            File.WriteAllText(scriptFile, script, Encoding.UTF8);

            // build pwsh
            var proc = new Process();
            var info = new ProcessStartInfo(this.PwshPath, $" -f \"{scriptFile}\"")
            {
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            proc.StartInfo = info;

            this.Log.LogCommandLine(MessageImportance.High,$"{info.FileName} {info.Arguments}");

            // run
            proc.Start();
            this.BuildEngine9.Yield();
            proc.WaitForExit();

            var code = proc.ExitCode;

            this.Log.LogMessageFromText($"Utopia.Tools: run [{info.FileName} {info.Arguments}] exit with code {code}", MessageImportance.High);

            if (code == 0)
            {
                this.Log.LogMessage("stdio:{0}", proc.StandardOutput.ReadToEnd());
                this.Log.LogMessage("stderr:{0}", proc.StandardError.ReadToEnd());
            }
            else
            {
                this.Log.LogError("stdio:{0}", proc.StandardOutput.ReadToEnd());
                this.Log.LogError("stderr:{0}", proc.StandardError.ReadToEnd());
            }

            proc.Dispose();

            return code == 0;
        }
    }
}
