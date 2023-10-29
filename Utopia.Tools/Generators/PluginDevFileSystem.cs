// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Text;
using Tomlyn;
using Utopia.Core.Utilities;
using Utopia.Core.Utilities.IO;

namespace Utopia.Tools.Generators;

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

    public const string DefaultPluginInfoFileName = "utopia.toml";

    public const string DefaultVersionFileName = "version.txt";

    string ProjectRootDir { get; }

    string TranslationDirectory { get; }

    string EntitiesDirectory { get; }

    string AssetsDirectory { get; }

    string PluginInfoFile { get; }

    string VersionFile { get; }

    string GeneratedDirectory { get; }

    string GetGeneratedCsFilePath(string origin)
    {
        origin = Path.GetFileName(origin);
        return Path.Join(GeneratedDirectory, $"{origin}.generated.cs");
    }
    string GetGeneratedCsFilePath(string origin, string path, string classify = "") => Path.Join(GeneratedDirectory, Path.GetRelativePath(origin, path) + $".{classify}.generated.cs");

    string GetTranslatedTomlFilePath(string origin)
    {
        origin = Path.GetFileName(origin);
        return Path.Join(TranslationDirectory, $"{origin}.translation.toml");
    }
    string GetTranslatedTomlFilePath(string origin, string path, string classify = "") => Path.Join(TranslationDirectory, Path.GetRelativePath(origin, path) + $".{classify}.translated.toml");

    string GetTranslatedTomlFilePath(TranslateItemType type) => GetTranslatedTomlFilePath(type.ToString());

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
        _ = sb.AppendLine("Target Plugin Information File:" + PluginInfoFile);

        return sb.ToString();
    }

    PluginInfo ReadPluginInfo(bool forceReread = false);
}

public class PluginDevFileSystem : IPluginDevFileSystem
{
    public string ProjectRootDir { get; set; }

    private PluginInfo? _info = null;

    public PluginInfo ReadPluginInfo(bool forceReread = false)
    {
        if (forceReread || _info == null)
        {
            string text = File.ReadAllText(PluginInfoFile, Encoding.UTF8);

            TomlModelOptions option = Guuid.AddTomlOption();

            _info = Toml.ToModel<PluginInfo>(text, options: option);
        }

        return _info;
    }

    public PluginDevFileSystem(string rootDir = ".")
    {
        ProjectRootDir = Path.GetFullPath(rootDir);
        TranslationDirectory = Path.Combine(ProjectRootDir, IPluginDevFileSystem.DefaultTranslationDirectoryName);
        EntitiesDirectory = Path.Combine(ProjectRootDir, IPluginDevFileSystem.DefaultEntitiesDirectoryName);
        AssetsDirectory = Path.Combine(ProjectRootDir, IPluginDevFileSystem.DefaultAssetsDirectoryName);
        PluginInfoFile = Path.Combine(ProjectRootDir, IPluginDevFileSystem.DefaultPluginInfoFileName);
        VersionFile = Path.Combine(ProjectRootDir, IPluginDevFileSystem.DefaultVersionFileName);
        GeneratedDirectory = Path.Combine(ProjectRootDir, IPluginDevFileSystem.DefaultGenerationDirectoryName);
    }

    public string TranslationDirectory { get; set; }

    public string EntitiesDirectory { get; set; }

    public string AssetsDirectory { get; set; }

    public string PluginInfoFile { get; set; }

    public string VersionFile { get; set; }

    public string GeneratedDirectory { get; set; }

    public override string ToString() => ((IPluginDevFileSystem)this).CreateString();
}

