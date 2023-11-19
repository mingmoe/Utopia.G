// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Text;
using System.Xml.Serialization;

namespace Utopia.Tools.Generators.Server;

public class ServerGeneratorConfiguration
{
    [XmlElement]
    public string ServerNamespaceName { get; set; } = "global";

    [XmlElement]
    public string? ServerPluginInformationClass { get; set; } = null;
}

/// <summary>
/// Generate the plugin file.
/// </summary>
public class PluginGenerator : IGenerator
{
    public string SubcommandName => "ServerPlugin";

    public void Execute(GeneratorOption option)
    {
        var source = GeneratorTemplate.ServerPluginClassTemplate
            .Replace("$TARGET_NAMESPACE$",
            option.Configuration.ServerGeneratorConfiguration.ServerNamespaceName)
            .Replace("$PluginInformationNamespace$",
            option.Configuration.ServerGeneratorConfiguration.ServerPluginInformationClass != null
            ? $"using {option.Configuration.ServerGeneratorConfiguration.ServerPluginInformationClass};" : string.Empty)
            .Replace("$GENERATOR_NAME$",typeof(PluginGenerator).FullName)
            .Replace("$GENERATOR_VERSION$",Program.GetVersion().ToString());

        string output = option.CurrentFileSystem.GetGeneratedCsFilePath("Plugin");

        File.WriteAllText(output, source, Encoding.UTF8);
    }
}
