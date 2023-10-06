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
    public IFileSystem TargetProject { get; set; } = null!;

    public string RootNamespace { get; set; } = null!;

    public void Execute()
    {
        // read toml
        this.TargetProject.CreateNotExistsDirectory();
        var text = File.ReadAllText(this.TargetProject.PluginInfoFile, System.Text.Encoding.UTF8);
        var version = File.ReadAllText(this.TargetProject.VersionFile, System.Text.Encoding.UTF8);

        var info = Toml.ToModel<PluginInfo>(text);

        var builder = new CsBuilder();

        builder.Usings.Add("Utopia.Core");
        builder.Usings.Add("Utopia.Core.Translate");
        builder.Usings.Add("Utopia.Core.Utilities");
        builder.Namespace = this.RootNamespace;

        builder.Lines.Add("public class PluginInformation : IPluginInformation{");
        builder.Lines.Add($"\tpublic readonly static Guuid ID = Guuid.ParseString(\"{info.Id}\");");
        builder.Lines.Add($"\tpublic readonly static TranslatedString NAME = new(\"{info.Name}\");");
        builder.Lines.Add($"\tpublic readonly static TranslatedString DESC = new(\"{info.Description}\");");
        builder.Lines.Add($"\tpublic readonly static System.Version VER = Version.Parse(\"{version}\");");
        builder.Lines.Add($"\tITranslatedString IPluginInformation.Name => NAME;");
        builder.Lines.Add($"\tITranslatedString IPluginInformation.Description => DESC;");
        builder.Lines.Add($"\tGuuid IPluginInformation.Id => ID;");
        builder.Lines.Add($"\tVersion IPluginInformation.Version => VER;");
        builder.Lines.Add($"\tstring IPluginInformation.License => \"{info.License}\";");
        builder.Lines.Add($"\tstring IPluginInformation.Homepage => \"{info.Homepage}\";");
        builder.Lines.Add("}");

        File.WriteAllText(this.TargetProject.GetGeneratedCsFileName(this.TargetProject.PluginInfoFile), builder.Generate(), System.Text.Encoding.UTF8);
    }
}
