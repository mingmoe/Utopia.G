// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license


// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

namespace Utopia.Core.IO;

/// <summary>
/// Like <see cref="IFileSystem"/>,but it is used for plugins in runtime.
/// </summary>
public interface IPluginFileSystem
{
    public const string DefaultTranslationDirectoryName = "Translation";

    public const string DefaultAssetsDirectoryName = IFileSystem.DefaultAssetsDirectoryName;

    /// <summary>
    /// If this plugins was extracted from the packed file,it refer to the packed file.
    /// </summary>
    string? PackedFile { get; }

    /// <summary>
    /// The directory path of the (extracted) plugin.
    /// You should not write anything under the directory.
    /// </summary>
    string RootDirectory { get; }

    /// <summary>
    /// The assembly path of the plugin.
    /// If the plugin is from memory,it's null.
    /// </summary>
    string? AssemblyFile { get; }

    /// <summary>
    /// Transition files path.
    /// </summary>
    string TranslationDirectory => Path.Join(RootDirectory, DefaultTranslationDirectoryName);

    /// <summary>
    /// Assets files path.
    /// </summary>
    string AssetsDirectory => Path.Join(RootDirectory, DefaultAssetsDirectoryName);

    string ConfigurationDirectory { get; }

    string GetConfigurationFilePathOfPlugin(string relativePathToConfig)
    {
        return Path.GetFullPath(Path.Join(ConfigurationDirectory, relativePathToConfig));
    }
}
