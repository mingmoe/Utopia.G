// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.Build.Framework;

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
            string formatted = str.Replace("\"", "`\"");
            formatted = formatted.Replace("'", "`\\'");
            return formatted;
        }

        public override bool Execute()
        {
            var sb = new StringBuilder();

            // find tools
            string tool;

            if (!string.IsNullOrEmpty(RunFromProject))
            {
                tool = _EscapePwsh(DotnetPath);
                _ = sb.Append($" run  --configuration \"{_EscapePwsh(ProjectCondifuration)}\" --project \"{_EscapePwsh(RunFromProject)}\" -- ");
            }
            else
            {
                if (!LocalTool)
                {
                    tool = _EscapePwsh("Utopia.Tools");

                }
                else
                {
                    tool = _EscapePwsh(DotnetPath);
                    _ = sb.Append(" tool run Utopia.Tools ");
                }
            }

            // build argument
            foreach (string argument in Arguments)
            {
                _ = sb.Append($" \"{_EscapePwsh(argument)}\" ");
            }

            var commentBuilder = new StringBuilder();
            foreach (string argument in Arguments)
            {
                commentBuilder.AppendLine($"# {argument.Replace("\n","`n")}");
            }

            // build script file
            string scriptFile = Path.GetTempFileName() + ".ps1";

            while (File.Exists(scriptFile))
            {
                scriptFile = Path.GetTempFileName() + ".ps1";
            }
            string script = $"\n{commentBuilder}\n&\"{tool}\" {sb}\n";

            Log.LogMessage(MessageImportance.High, $"generate powershell script file({scriptFile}):{script}");

            File.WriteAllText(scriptFile, script, Encoding.UTF8);

            // build pwsh process
            var proc = new Process();
            var info = new ProcessStartInfo(PwshPath, $" -f \"{scriptFile}\"")
            {
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            proc.StartInfo = info;

            Log.LogCommandLine(MessageImportance.High, $"{info.FileName} {info.Arguments}");

            // run
            _ = proc.Start();
            proc.WaitForExit();

            int code = proc.ExitCode;

            _ = Log.LogMessageFromText($"Utopia.Tools: run [{info.FileName} {info.Arguments}] exit with code {code}", MessageImportance.High);

            if (code == 0)
            {
                Log.LogMessage("stdio:{0}", proc.StandardOutput.ReadToEnd());
                Log.LogMessage("stderr:{0}", proc.StandardError.ReadToEnd());
            }
            else
            {
                Log.LogError("stdio:{0}", proc.StandardOutput.ReadToEnd());
                Log.LogError("stderr:{0}", proc.StandardError.ReadToEnd());
            }

            proc.Dispose();

            return code == 0;
        }
    }
}
