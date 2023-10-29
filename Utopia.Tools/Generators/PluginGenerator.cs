// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Text;

namespace Utopia.Tools.Generators;

/// <summary>
/// Generate the plugin file.
/// </summary>
public class PluginGenerator : IGenerator
{
    public string SubcommandName => "server-plugin";

    public void Execute(GeneratorOption option)
    {
        var source = GeneratorTemplate.PluginClassTemplate.Replace("$TARGET_NAMESPACE$", option.TargetNamespace);
       
        string output = option.TargetProject.GetGeneratedCsFilePath("Plugin");

        File.WriteAllText(output, source, Encoding.UTF8);
    }
}
