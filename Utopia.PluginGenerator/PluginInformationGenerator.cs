using Microsoft.CodeAnalysis;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Tomlyn;
using Utopia.Generator;

namespace Utopia.PluginGenerator;

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
[Generator]
public class PluginInformationGenerator : ISourceGenerator
{

    public IFileSystem FileSystem { get; set; } = null!;

    public void Execute(GeneratorExecutionContext context)
    {
        try
        {
            this.FileSystem = new FileSystem(context);

            // read toml
            var reader = context.GenerateFileSystem();
            var text = reader.TryReadFile(FileSystem.PluginInfoFile);


            if (text == null)
            {
                context.ReportWarning("failed to read plugin information file:{0}", FileSystem.PluginInfoFile);
                return;
            }

            var info = Toml.ToModel<PluginInfo>(text);

            context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.RootNamespace", out var rootNamespace);
            context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.Version", out var version);

            if (rootNamespace == null || version == null)
            {
                context.ReportWarning("failed to get the root namespace or version. add {0} to your project file",
                """
            <ItemGroup>
                <CompilerVisibleProperty Include="RootNamespace" />
                <CompilerVisibleProperty Include="Version" />
            </ItemGroup>
        """);
                return;
            }

            var builder = new CsGenerator();

            builder.Usings.Add("Utopia.Core");
            builder.Usings.Add("Utopia.Core.Translate");
            builder.Usings.Add("Utopia.Core.Utilities");
            builder.Namespace = rootNamespace;

            builder.Lines.Add("public class PluginInformation : IPluginInformation{");
            builder.Lines.Add($"public readonly static Guuid ID = Guuid.ParseString(\"{info.Id}\");");
            builder.Lines.Add($"public readonly static TranslatedString NAME = new(\"{info.Name}\");");
            builder.Lines.Add($"public readonly static TranslatedString DESC = new(\"{info.Description}\");");
            builder.Lines.Add($"public readonly static System.Version VER = Version.Parse(\"{version}\");");
            builder.Lines.Add($"ITranslatedString IPluginInformation.Name => NAME;");
            builder.Lines.Add($"ITranslatedString IPluginInformation.Description => DESC;");
            builder.Lines.Add($"Guuid IPluginInformation.Id => ID;");
            builder.Lines.Add($"Version IPluginInformation.Version => VER;");
            builder.Lines.Add($"string IPluginInformation.License => \"{info.License}\";");
            builder.Lines.Add($"string IPluginInformation.Homepage => \"{info.Homepage}\";");
            builder.Lines.Add("}");

            context.AddSource(this.FileSystem.GetGeneratedFileName(FileSystem.PluginInfoFile), builder.Generate());
        }
        catch(Exception ex)
        {
            context.ReportError("got a exception:{0}", ex.ToString());
            Console.WriteLine(ex.ToString());

            if(ex.InnerException != null)
            {
                throw ex.InnerException;
            }
            throw;
        }
    }

    public void Initialize(GeneratorInitializationContext context)
    {

    }
}
