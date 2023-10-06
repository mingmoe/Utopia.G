using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utopia.Generator;

/// <summary>
/// This class hint which file should be read.
/// And where the file is.
/// </summary>
public interface IFileSystem
{
    string ProjectRootDir { get; }

    string TranslateDir { get; }

    string EntitiesDir { get; }

    string AssertDir { get; }

    string PluginInfoFile { get; }

    string GetGeneratedFileName(string origin);
}

public class FileSystem : IFileSystem
{
    public string ProjectRootDir { get; set; }

    public FileSystem(string rootDir = ".")
    {
        this.ProjectRootDir = Path.GetFullPath(rootDir);
        this.TranslateDir = Path.Combine(this.ProjectRootDir, "translate");
        this.EntitiesDir = Path.Combine(this.ProjectRootDir, "entities");
        this.AssertDir = Path.Combine(this.ProjectRootDir, "assert");
        this.PluginInfoFile = Path.Combine(this.ProjectRootDir, "utopia.toml");
    }

    public FileSystem(GeneratorExecutionContext context) : this(Utilities.GetProjectDir(context) ?? ".") {

    }

    public string TranslateDir { get; set; }

    public string EntitiesDir { get; set; }

    public string AssertDir { get; set; }

    public string PluginInfoFile { get; set; }

    public string GetGeneratedFileName(string origin)
    {
        origin = Path.GetFileName(origin);
        return $"{origin}.generated.cs";
    }
}
