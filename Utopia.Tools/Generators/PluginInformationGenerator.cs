using NLog;
using Tomlyn;

namespace Utopia.Tools.Generators;

public class PluginInfo
{
    public string Name { get; set; } = "Unknown";

    public string Description { get; set; } = "Unknown";

    public string License { get; set; } = "Unknown";

    public string Id { get; set; } = "unknown.unknown";

    public string Homepage { get; set; } = "Unknown";
}

/// <summary>
/// This generator will generate the `Plugin` class with information
/// from the file.
/// </summary>
public class PluginInformationGenerator : IGenerator
{

    private readonly static Logger _logger = LogManager.GetCurrentClassLogger();

    public void Execute(GeneratorOption option)
    {
        // read toml
        option.TargetProject.CreateNotExistsDirectory();

        var output = option.TargetProject.GetGeneratedCsFilePath(option.TargetProject.PluginInfoFile);
        var inputPlugin = option.TargetProject.PluginInfoFile;
        var inputVersion = option.TargetProject.VersionFile;

        var text = File.ReadAllText(inputPlugin, System.Text.Encoding.UTF8);
        var version = File.ReadAllText(inputVersion, System.Text.Encoding.UTF8);

        var info = Toml.ToModel<PluginInfo>(text);

        var builder = new CsBuilder();

        builder.Usings.Add("Utopia.Core");
        builder.Usings.Add("Utopia.Core.Translate");
        builder.Usings.Add("Utopia.Core.Utilities");
        builder.Namespace = option.TargetNamespace;

        builder.AddClass("PluginInformation",isPublic: true,parentClass: "IPluginInformation",
            from: $"this file was generated from {option.TargetProject.PluginInfoFile}");
        builder.AddField("public", "Guuid", "ID", $"Guuid.ParseString(\"{info.Id}\")", true, true);
        builder.AddField("public", "TranslatedString", "NAME", $"new(\"{info.Name}\")", true, true);
        builder.AddField("public", "TranslatedString", "DESC", $"new(\"{info.Description}\")", true, true);
        builder.AddField("public", "System.Version", "VER", $"Version.Parse(\"{version}\")", true, true);
        builder.AddLine($"ITranslatedString IPluginInformation.Name => NAME;");
        builder.AddLine($"ITranslatedString IPluginInformation.Description => DESC;");
        builder.AddLine($"Guuid IPluginInformation.Id => ID;");
        builder.AddLine($"Version IPluginInformation.Version => VER;");
        builder.AddLine($"string IPluginInformation.License => \"{info.License}\";");
        builder.AddLine($"string IPluginInformation.Homepage => \"{info.Homepage}\";");
        builder.CloseCodeBlock();

        File.WriteAllText(output, builder.Generate(),
            System.Text.Encoding.UTF8);
    }
}
