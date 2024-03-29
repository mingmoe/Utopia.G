// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Text;
using System.Xml.Serialization;
using Microsoft.CodeAnalysis.CSharp;
using Utopia.Core.Utilities;

namespace Utopia.Tool.Generators;

public class PluginInformation
{
    [XmlElement]
    public string Name { get; set; } = "Unknown";

    [XmlElement]
    public string Description { get; set; } = "Unknown";

    [XmlElement]
    public string License { get; set; } = "Unknown";

    [XmlElement]
    public XmlGuuid Id { get; set; } = new XmlGuuid();

    [XmlElement]
    public string Homepage { get; set; } = "Unknown";

    [XmlArray("Dependences")]
    [XmlArrayItem("Dependence")]
    public XmlGuuid[] Dependences { get; set; } = [];
}

/// <summary>
/// This generator will generate the `Plugin` class with information
/// from the file.
/// </summary>
public class PluginInformationGenerator : IGenerator
{
    public string SubcommandName => "PluginInformation";

    public void Execute(GeneratorOption option)
    {
        string output = option.CurrentFileSystem.GetGeneratedCsFilePath(IPluginDevFileSystem.DefaultConfigurationFileName);
        string inputVersion = option.CurrentFileSystem.VersionFile;
        var info = option.Configuration.PluginInformation;

        string version = File.ReadAllText(inputVersion, Encoding.UTF8).Trim();

        StringBuilder sb = new();
        foreach (var dep in info.Dependences)
        {
            // the last ',' will be ignored by C#
            sb.Append($"Guuid.Parse({SymbolDisplay.FormatLiteral(dep.Guuid.ToString(), true)}),");
        }
        string deps = sb.ToString();

        var builder = new CsBuilder(null, inputVersion);

        builder.Using.Add("Utopia.Core.Plugin");
        builder.Using.Add("Utopia.Core.Utilities");
        builder.Namespace = option.CurrentProject.Configuration.RootNamespace;

        builder.EmitClass("PluginInformation", isPublic: true, addGeneratedCodeAttribute: true, parentClass: ["IPluginInformation"]);
        builder.EmitField("public", "Guuid", "ID", $"Guuid.Parse({SymbolDisplay.FormatLiteral(info.Id.ToString(), true)})", true, true);
        builder.EmitField("public", "string", "NAME", $"{SymbolDisplay.FormatLiteral(info.Name, true)}", true, true);
        builder.EmitField("public", "string", "DESC", $"{SymbolDisplay.FormatLiteral(info.Description, true)}", true, true);
        builder.EmitField("public", "System.Version", "VER", $"Version.Parse({SymbolDisplay.FormatLiteral(version,true)})", true, true);
        builder.EmitField("public", "string", "HOMEPAGE", $"{SymbolDisplay.FormatLiteral(info.Homepage, true)}", true, true);
        builder.EmitField("public", "string", "LICENSE", $"{SymbolDisplay.FormatLiteral(info.License, true)}", true, true);
        builder.EmitField("public", "Guuid[]", "DEPENDENCES", $"[{deps}]", true, true);
        builder.EmitLine($"string IPluginInformation.Name => NAME;");
        builder.EmitLine($"string IPluginInformation.Description => DESC;");
        builder.EmitLine($"Guuid IPluginInformation.Id => ID;");
        builder.EmitLine($"Version IPluginInformation.Version => VER;");
        builder.EmitLine($"string IPluginInformation.License => LICENSE;");
        builder.EmitLine($"string IPluginInformation.Homepage => HOMEPAGE;");
        builder.EmitLine($"Guuid[] IPluginInformation.Dependences => DEPENDENCES;");
        builder.CloseCodeBlock();

        File.WriteAllText(output, builder.Generate(),
            Encoding.UTF8);
    }
}
