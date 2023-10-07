namespace Utopia.Tools.Generators;

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

    string VersionFile { get; }

    string GeneratedDir { get; }

    string GetGeneratedCsFilePath(string origin)
    {
        origin = Path.GetFileName(origin);
        return Path.Join(this.GeneratedDir, $"{origin}.generated.cs");
    }
    string GetGeneratedCsFilePath(string origin,string path,string classify = "")
    {
        return Path.Join(this.GeneratedDir,Path.GetRelativePath(origin,path) + $".{classify}.generated.cs");
    }

    string GetTranslatedTomlFilePath(string origin)
    {
        origin = Path.GetFileName(origin);
        return Path.Join(this.TranslateDir, $"{origin}.translated.toml");
    }
    string GetTranslatedTomlFilePath(string origin, string path, string classify = "")
    {
        return Path.Join(this.TranslateDir, Path.GetRelativePath(origin, path) + $".{classify}.translated.toml");
    }

    void CreateNotExistsDirectory()
    {
        Directory.CreateDirectory(this.TranslateDir);
        Directory.CreateDirectory(this.EntitiesDir);
        Directory.CreateDirectory(this.AssertDir);
        Directory.CreateDirectory(this.GeneratedDir);
    }
}

public class FileSystem : IFileSystem
{
    public string ProjectRootDir { get; set; }

    public FileSystem(string rootDir = ".")
    {
        this.ProjectRootDir = Path.GetFullPath(rootDir);
        this.TranslateDir = Path.Combine(this.ProjectRootDir, "Translate");
        this.EntitiesDir = Path.Combine(this.ProjectRootDir, "Entities");
        this.AssertDir = Path.Combine(this.ProjectRootDir, "Assert");
        this.PluginInfoFile = Path.Combine(this.ProjectRootDir, "utopia.toml");
        this.VersionFile = Path.Combine(this.ProjectRootDir, "version.txt");
        this.GeneratedDir = Path.Combine(this.ProjectRootDir, "Generated");
    }

    public string TranslateDir { get; set; }

    public string EntitiesDir { get; set; }

    public string AssertDir { get; set; }

    public string PluginInfoFile { get; set; }

    public string VersionFile { get; set; }

    public string GeneratedDir { get; set; }
}

