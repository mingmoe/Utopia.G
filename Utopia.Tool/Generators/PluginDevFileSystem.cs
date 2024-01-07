// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Text;
using Utopia.Core.IO;
using Utopia.Core.Utilities;

namespace Utopia.Tool.Generators;

/// <summary>
/// This class hint which file should be read.
/// And where the file is.
/// </summary>
public interface IPluginDevFileSystem
{
    public const string DefaultGenerationDirectoryName = "Generated";

    public const string DefaultTranslationDirectoryName = IPluginFileSystem.DefaultTranslationDirectoryName;

    public const string DefaultAssetsDirectoryName = IPluginFileSystem.DefaultAssetsDirectoryName;

    public const string DefaultEntitiesDirectoryName = "Entities";

    public const string DefaultConfigurationFileName = "utopia.xml";

    public const string DefaultVersionFileName = "version.txt";

    string ProjectRootDir { get; }

    string TranslationDirectory { get; }

    string EntitiesDirectory { get; }

    string AssetsDirectory { get; }

    string VersionFile { get; }

    string GeneratedDirectory { get; }

    string GetGeneratedCsFilePath(string origin)
    {
        origin = Path.GetFileName(origin);
        return Path.Join(GeneratedDirectory, $"{origin}.generated.cs");
    }
    string GetGeneratedCsFilePath(string origin, string path, string classify = "") => Path.Join(GeneratedDirectory, Path.GetRelativePath(origin, path) + $".{classify}.generated.cs");

    void CreateNotExistsDirectory()
    {
        _ = Directory.CreateDirectory(TranslationDirectory);
        _ = Directory.CreateDirectory(EntitiesDirectory);
        _ = Directory.CreateDirectory(AssetsDirectory);
        _ = Directory.CreateDirectory(GeneratedDirectory);
    }

    string CreateString()
    {
        var sb = new StringBuilder();

        _ = sb.AppendLine("Target Project Root:" + ProjectRootDir);
        _ = sb.AppendLine("Target Entities:" + EntitiesDirectory);
        _ = sb.AppendLine("Target Asserts:" + AssetsDirectory);
        _ = sb.AppendLine("Target Generated:" + GeneratedDirectory);
        _ = sb.AppendLine("Target Translate:" + TranslationDirectory);
        _ = sb.AppendLine("Target Version File:" + VersionFile);

        return sb.ToString();
    }
}

public class PluginDevFileSystem : IPluginDevFileSystem
{
    public PluginDevFileSystem(string rootDir = ".")
    {
        ProjectRootDir = Path.GetFullPath(rootDir);
        TranslationDirectory = Path.Combine(ProjectRootDir, IPluginDevFileSystem.DefaultTranslationDirectoryName);
        EntitiesDirectory = Path.Combine(ProjectRootDir, IPluginDevFileSystem.DefaultEntitiesDirectoryName);
        AssetsDirectory = Path.Combine(ProjectRootDir, IPluginDevFileSystem.DefaultAssetsDirectoryName);
        VersionFile = Path.Combine(ProjectRootDir, IPluginDevFileSystem.DefaultVersionFileName);
        GeneratedDirectory = Path.Combine(ProjectRootDir, IPluginDevFileSystem.DefaultGenerationDirectoryName);
    }

    public string ProjectRootDir { get; set; }

    public string TranslationDirectory { get; set; }

    public string EntitiesDirectory { get; set; }

    public string AssetsDirectory { get; set; }

    public string VersionFile { get; set; }

    public string GeneratedDirectory { get; set; }

    public override string ToString() => ((IPluginDevFileSystem)this).CreateString();
}

