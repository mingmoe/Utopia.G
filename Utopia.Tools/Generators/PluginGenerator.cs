using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utopia.Tools.Generators;

/// <summary>
/// Generate the plugin file.
/// </summary>
public class PluginGenerator : IGenerator
{
    public string SubcommandName => "plugin";

    public void Execute(GeneratorOption option)
    {
        CsBuilder builder = new()
        {
            Namespace = option.TargetNamespace
        };
        builder.Usings.Add("Utopia.Server");
        builder.Usings.Add("Utopia.Core");
        builder.Usings.Add("Autofac");

        builder.AddClass("Plugin",
             isPublic: true, isStatic: false, isPartial: true, "IPlugin", "PluginInformation");

        builder.AddField("private", "Core.IServiceProvider","_provider",isReadonly: true);
        builder.AddField("private", "ILifetimeScope", "_container", isReadonly: true);

        builder.AddLine("public Plugin(Core.IServiceProvider provider)");
        builder.BeginCodeBlock();
        builder.AddLine("ArgumentNullException.ThrowIfNull(provider);");
        builder.AddLine("this._provider = provider;");
        builder.AddLine("var container = provider.GetService<IContainer>();");
        builder.AddLine("var scope = container.BeginLifetimeScope((builder) =>");
        builder.BeginCodeBlock();
        builder.AddLine(");");
        builder.AddLine("this._container = scope;");
        builder.CloseCodeBlock();
        builder.CloseCodeBlock();
        builder.CloseCodeBlock();

        var output = option.TargetProject.GetGeneratedCsFilePath("plugin");

        File.WriteAllText(output, builder.Generate(), Encoding.UTF8);
    }
}
