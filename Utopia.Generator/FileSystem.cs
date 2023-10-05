using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utopia.Generator;

/// <summary>
/// This class stands for file system.
/// </summary>
public interface IFileSystem
{
    string ProjectRootDir { get; }

    string TranslateDir { get; }

    string EntitiesDir { get; }

    string AssertDir { get; }

    string PluginInfoFile { get; }
}

public class FileSystem : IFileSystem
{
    public string ProjectRootDir { get; init; }

    public FileSystem(string rootDir = ".")
    {
        this.ProjectRootDir = Path.GetFullPath(rootDir);
        this.TranslateDir = Path.Join(this.ProjectRootDir, "translate");
        this.EntitiesDir = Path.Join(this.ProjectRootDir, "entities");
        this.AssertDir = Path.Join(this.ProjectRootDir, "assert");
        this.PluginInfoFile = Path.Join(this.ProjectRootDir, "utopia.toml");
    }

    public string TranslateDir { get; set; }

    public string EntitiesDir { get; set; }

    public string AssertDir { get; set; }

    public string PluginInfoFile { get; set; }
}
