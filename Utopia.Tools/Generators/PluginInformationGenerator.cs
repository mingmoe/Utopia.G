// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using Utopia.Core.Utilities;

namespace Utopia.Tools.Generators;

public class PluginInfo
{
    public string Name { get; set; } = "Unknown";

    public string Description { get; set; } = "Unknown";

    public string License { get; set; } = "Unknown";

    public Guuid Id { get; set; } = Guuid.Empty;

    public string Homepage { get; set; } = "Unknown";
}

/// <summary>
/// This generator will generate the `Plugin` class with information
/// from the file.
/// </summary>
public class PluginInformationGenerator : IGenerator
{
    public string SubcommandName => "plugin-info";

    public void Execute(GeneratorOption option)
    {
        // read toml
        string output = option.TargetProject.GetGeneratedCsFilePath(option.TargetProject.PluginInfoFile);
        string inputPlugin = option.TargetProject.PluginInfoFile;
        string inputVersion = option.TargetProject.VersionFile;

        string version = File.ReadAllText(inputVersion, System.Text.Encoding.UTF8).Trim();

        PluginInfo info = option.TargetProject.ReadPluginInfo();

        var builder = new CsBuilder(inputPlugin, inputVersion);

        builder.Usings.Add("Utopia.Core.Plugin");
        builder.Usings.Add("Utopia.Core.Transition");
        builder.Usings.Add("Utopia.Core.Utilities");
        builder.Namespace = option.TargetNamespace;

        builder.EmitClass("PluginInformation", isPublic: true, parentClass: "IPluginInformation");
        builder.EmitField("public", "Guuid", "ID", $"Guuid.Parse(\"{info.Id}\")", true, true);
        builder.EmitField("public", "TranslatedString", "NAME", $"new(\"{info.Name}\")", true, true);
        builder.EmitField("public", "TranslatedString", "DESC", $"new(\"{info.Description}\")", true, true);
        builder.EmitField("public", "System.Version", "VER", $"Version.Parse(\"{version}\")", true, true);
        builder.EmitField("public", "string", "HOMEPAGE", $"\"{info.Homepage}\"", true, true);
        builder.EmitField("public", "string", "LICENSE", $"\"{info.License}\"", true, true);
        builder.EmitLine($"ITranslatedString IPluginInformation.Name => NAME;");
        builder.EmitLine($"ITranslatedString IPluginInformation.Description => DESC;");
        builder.EmitLine($"Guuid IPluginInformation.Id => ID;");
        builder.EmitLine($"Version IPluginInformation.Version => VER;");
        builder.EmitLine($"string IPluginInformation.License => LICENSE;");
        builder.EmitLine($"string IPluginInformation.Homepage => HOMEPAGE;");
        builder.CloseCodeBlock();

        File.WriteAllText(output, builder.Generate(),
            System.Text.Encoding.UTF8);

        option.TranslateManager.EnsurePluginInformationTrnaslate();
    }
}
