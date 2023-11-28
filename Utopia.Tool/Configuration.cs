// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Utopia.Core;
using Utopia.Tools.Generators;
using Utopia.Tools.Generators.Server;

namespace Utopia.Tools;

public class ProjectConfiguration
{
    [XmlElement]
    public string RootNamespace { get; set; } = "global";

    [XmlElement]
    public string? VersionFile { get; set; } = null;

    [XmlElement]
    public string AssetsDirectory { get; set; } = IPluginDevFileSystem.DefaultAssetsDirectoryName;

    [XmlElement]
    public string TransitionDirectory { get; set; } = IPluginDevFileSystem.DefaultTranslationDirectoryName;

    [XmlElement]
    public string GeneratedDirectory { get; set; } = IPluginDevFileSystem.DefaultGenerationDirectoryName;

    [XmlElement]
    public string EntitiesDirectory { get; set; } = IPluginDevFileSystem.DefaultEntitiesDirectoryName;

    public void ApplyToFileSystem(PluginDevFileSystem system,string backupVersionFile)
    {
        system.AssetsDirectory = AssetsDirectory;
        system.TranslationDirectory = TransitionDirectory;
        system.EntitiesDirectory = EntitiesDirectory;
        system.VersionFile = VersionFile ?? backupVersionFile;
        system.GeneratedDirectory = GeneratedDirectory;
    }
}

public class SubprojectConfiguration
{
    [XmlElement]
    public string Path { get; set; } = ".";

    [XmlArray("Generators")]
    [XmlArrayItem("Generator")]
    public List<string> Generators { get; set; } = [];

    [XmlElement]
    public ProjectConfiguration Configuration { get; set; } = new();
}

/// <summary>
/// This class is a configuration.
/// </summary>
[XmlRoot(Namespace = Xml.Namespace)]
public class Configuration
{
    /// <summary>
    /// If this is null,we set the directory where the configuration file is to root.
    /// </summary>
    [XmlElement]
    public string? RootDirectory { get; set; } = null;

    /// <summary>
    /// This file is solved under <see cref="RootDirectory"/>.
    /// </summary>
    [XmlElement]
    public string VersionFile { get; set; } = IPluginDevFileSystem.DefaultVersionFileName;

    /// <summary>
    /// What do you want to generate?
    /// </summary>
    [XmlArray("Subprojects")]
    [XmlArrayItem("Subproject")]
    public List<SubprojectConfiguration> Subprojects { get; set; } = [];

    [XmlElement]
    public TranslationConfiguration TranslationConfiguration { get; set; } = new();

    [XmlElement]
    public TranslationProviderConfiguration TranslationProviderConfiguration { get; set; } = new();

    [XmlElement]
    public ServerGeneratorConfiguration ServerGeneratorConfiguration { get; set; } = new();

    [XmlElement]
    public PluginInformation PluginInformation { get; set; } = new();

    [XmlElement(IsNullable = true)]
    public string? GenerateXmlSchemaFileTo { get; set; } = null;
}
