// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Text;
using System.Xml.Serialization;

namespace Utopia.Tool.Generators.Server;

public class ServerPluginClassConfiguration
{
    [XmlElement]
    public string ServerNamespaceName { get; set; } = "global";

    [XmlElement]
    public string? PluginInformationClassNamespace { get; set; } = null;
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
            option.Configuration.ServerPluginClassConfiguration.ServerNamespaceName)
            .Replace("$PluginInformationNamespace$",
            option.Configuration.ServerPluginClassConfiguration.PluginInformationClassNamespace != null
            ? $"using {option.Configuration.ServerPluginClassConfiguration.PluginInformationClassNamespace};" : string.Empty)
            .Replace("$GENERATOR_NAME$", typeof(PluginGenerator).FullName)
            .Replace("$GENERATOR_VERSION$", Program.GetVersion().ToString());

        string output = option.CurrentFileSystem.GetGeneratedCsFilePath("Plugin");

        File.WriteAllText(output, source, Encoding.UTF8);
    }
}
