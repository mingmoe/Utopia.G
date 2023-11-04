// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Utopia.Tools.Generators;
using Utopia.Tools.Generators.Server;

namespace Utopia.Tools;

/// <summary>
/// This class is a configuration.
/// </summary>
public class Configuration
{
    [XmlElement]
    public string RootNamespace { get; set; } = "global";

    [XmlElement]
    public string VersionFile { get; set; } = IPluginDevFileSystem.DefaultVersionFileName;

    [XmlElement]
    public string AssetsDirectory { get; set; } = IPluginDevFileSystem.DefaultAssetsDirectoryName;

    [XmlElement]
    public string TransitionDirectory { get; set; } = IPluginDevFileSystem.DefaultTranslationDirectoryName;

    [XmlElement]
    public string GeneratedDirectory { get; set; } = IPluginDevFileSystem.DefaultGenerationDirectoryName;

    [XmlElement]
    public string EntitiesDirectory { get; set; } = IPluginDevFileSystem.DefaultEntitiesDirectoryName;

    [XmlElement]
    public string ProjectRootDir { get; set; } = ".";

    /// <summary>
    /// What do you want to generate?
    /// </summary>
    [XmlArray("Generators")]
    [XmlArrayItem("Generator")]
    public List<string> Generators { get; set; } = [];

    [XmlElement]
    public TransitionConfiguration TransitionConfiguration { get; set; } = new();

    [XmlElement]
    public ServerGeneratorConfiguration ServerGeneratorConfiguration { get; set; } = new();

    [XmlElement]
    public PluginInformation Plugin { get; set; } = new();

    public void ApplyToFileSystem(PluginDevFileSystem system)
    {
        system.AssetsDirectory = AssetsDirectory;
        system.TranslationDirectory = TransitionDirectory;
        system.ProjectRootDir = ProjectRootDir;
        system.EntitiesDirectory = EntitiesDirectory;
        system.VersionFile = VersionFile;
        system.GeneratedDirectory = GeneratedDirectory;
    }
}
