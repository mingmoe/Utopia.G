using Microsoft.CodeAnalysis;
using System;

namespace Utopia.PluginGenerator;

/// <summary>
/// This generator will generate the `Plugin` class with information
/// from the file.
/// </summary>
[Generator]
public class PluginInformationGenerator : ISourceGenerator
{

    public const string PLUGIN_FILE = "utopia.toml";

    public static string? GetCallingPath(GeneratorExecutionContext context)
    {
        return context.AnalyzerConfigOptions.GlobalOptions
            .TryGetValue("build_property.projectdir", out var result) ? result : null;
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var d = new DiagnosticDescriptor(id: "Error",
                                                                                   title: "Code Generator Error",
                                                                                   messageFormat: "{0}",
                                                                                   category: "CodeGenerator",
                                                                                   DiagnosticSeverity.Error,
                                                                                   isEnabledByDefault: true);
        context.ReportDiagnostic(Diagnostic.Create(d, Location.None,GetCallingPath(context) ?? string.Empty));

    }

    public void Initialize(GeneratorInitializationContext context)
    {
        
    }
}
