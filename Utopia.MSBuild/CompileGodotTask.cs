using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace Utopia.MSBuild
{
    public class CompileGodotTask : Task
    {
        [Required]
        public string GodotExecuteFilePath { get; set; }

        [Required]
        public string ExportConfigFilePath { get; set; }

        [Required]
        public string ExportProjectDirectoryPath { get; set; }

        [Required]
        public string ExportPath { get; set; }

        [Required]
        public ITaskItem[] Arguments { get; set; }

        /// <summary>
        /// see:
        /// https://docs.godotengine.org/en/stable/tutorials/editor/command_line_tutorial.html#:~:text=%2D%2Dscript).-,%2D%2Dexport%2Drelease,-%3Cpreset%3E%20%3Cpath
        /// </summary>
        public string ExportMode { get; set; } = "release";

        public override bool Execute()
        {
            if (this.ExportMode != "release" && this.ExportMode != "debug" && this.ExportMode != "pack")
            {
                this.Log.LogError("the export mode {mode} is wrong", this.ExportMode);
                return false;
            }

            var godot = Path.GetFullPath(this.GodotExecuteFilePath);

            if (!File.Exists(godot))
            {
                this.Log.LogError("failed to found godot file at {path}", godot);
                return false;
            }

            var godotProc = new Process();
            {
                var info = godotProc.StartInfo;
                info.UseShellExecute = false;
                info.RedirectStandardOutput = true;
                info.RedirectStandardError = true;
                info.RedirectStandardInput = false;
                info.WindowStyle = ProcessWindowStyle.Hidden;
                info.CreateNoWindow = true;
                info.FileName = godot;
                info.Arguments = string.Format(" --path {0} --headless --export-{1} {2} {3} --build-solutions",
                    this.ExportProjectDirectoryPath,
                    this.ExportMode,
                    this.ExportConfigFilePath,
                    this.ExportPath);

                this.Log.LogCommandLine(MessageImportance.High,
                    string.Format("{0} {1}", info.FileName, info.Arguments));
            }
            try
            {
                godotProc.Start();

                var standardBuilder = new StringBuilder();
                var errorBuilder = new StringBuilder();

                while (true)
                {
                    standardBuilder.Append(godotProc.StandardOutput.ReadToEnd());
                    errorBuilder.Append(godotProc.StandardError.ReadToEnd());

                    if (godotProc.HasExited)
                    {
                        standardBuilder.Append(godotProc.StandardOutput.ReadToEnd());
                        errorBuilder.Append(godotProc.StandardError.ReadToEnd());
                        break;
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }

                if (godotProc.ExitCode != 0)
                {
                    this.Log.LogMessage("godot failed,STDOUT:{0}\nSTDERR:{1}",
                        standardBuilder.ToString(),
                        errorBuilder.ToString());
                    throw new Exception("failed to execute godot");
                }
            }
            catch (Exception ex)
            {
                this.Log.LogErrorFromException(ex);
            }

            return true;
        }
    }
}
