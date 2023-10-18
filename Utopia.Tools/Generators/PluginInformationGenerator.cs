using NLog;
using Tomlyn;
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
        var output = option.TargetProject.GetGeneratedCsFilePath(option.TargetProject.PluginInfoFile);
        var inputPlugin = option.TargetProject.PluginInfoFile;
        var inputVersion = option.TargetProject.VersionFile;

        var version = File.ReadAllText(inputVersion, System.Text.Encoding.UTF8).Trim();

        var info = option.TargetProject.ReadPluginInfo();

        var builder = new CsBuilder(inputPlugin,inputVersion);

        builder.Usings.Add("Utopia.Core");
        builder.Usings.Add("Utopia.Core.Translate");
        builder.Usings.Add("Utopia.Core.Utilities");
        builder.Namespace = option.TargetNamespace;

        builder.AddClass("PluginInformation",isPublic: true,parentClass: "IPluginInformation");
        builder.AddField("public", "Guuid", "ID", $"Guuid.Parse(\"{info.Id}\")", true, true);
        builder.AddField("public", "TranslatedString", "NAME", $"new(\"{info.Name}\")", true, true);
        builder.AddField("public", "TranslatedString", "DESC", $"new(\"{info.Description}\")", true, true);
        builder.AddField("public", "System.Version", "VER", $"Version.Parse(\"{version}\")", true, true);
        builder.AddField("public", "string", "HOMEPAGE", $"\"{info.Homepage}\"", true, true);
        builder.AddField("public", "string", "LICENSE", $"\"{info.License}\"", true, true);
        builder.AddLine($"ITranslatedString IPluginInformation.Name => NAME;");
        builder.AddLine($"ITranslatedString IPluginInformation.Description => DESC;");
        builder.AddLine($"Guuid IPluginInformation.Id => ID;");
        builder.AddLine($"Version IPluginInformation.Version => VER;");
        builder.AddLine($"string IPluginInformation.License => LICENSE;");
        builder.AddLine($"string IPluginInformation.Homepage => HOMEPAGE;");
        builder.CloseCodeBlock();

        File.WriteAllText(output, builder.Generate(),
            System.Text.Encoding.UTF8);

        option.TranslateManager.EnsurePluginInformationTrnaslate();
    }
}
