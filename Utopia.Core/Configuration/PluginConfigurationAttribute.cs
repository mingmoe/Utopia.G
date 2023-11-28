// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using Utopia.Core.IO;

namespace Utopia.Core.Configuration;

/// <summary>
/// Don't include extension name.
/// e.g. use Config not Config.xml or Config.xsd
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class PluginConfigurationAttribute(string filePath, bool generateXsd = true, string? xsdFilePath = null) : Attribute
{
    /// <summary>
    /// Relative path to configuration file.
    /// For example, it may be relative to <see cref="IFileSystem.ConfigurationDirectory"/>
    /// or <see cref="IPluginFileSystem.ConfigurationDirectory"/>.
    /// </summary>
    public readonly string FilePath = filePath + ".xml";

    public readonly bool GenerateXsdFile = generateXsd;

    public readonly string XsdFilePath = (xsdFilePath ?? filePath) + ".xsd";
}

