using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.IO;

namespace Utopia.Generator;

public static class Utilities
{
    public static void ReportError(this GeneratorExecutionContext context, string msg, params object[] args)
    {
        var d = new DiagnosticDescriptor(
            id: "UtopiaSourceGeneratorError",
            title: "Utopia.SourceGenerator.Error",
                                                                                  messageFormat: msg,
                                                                                  category: "CodeGenerator",
                                                                                  DiagnosticSeverity.Error,
                                                                                  isEnabledByDefault: true);
        context.ReportDiagnostic(Diagnostic.Create(d, Location.None, args));
    }

    public static void ReportInfo(this GeneratorExecutionContext context, string msg, params object[] args)
    {
        var d = new DiagnosticDescriptor(id: "UtopiaSourceGeneratorInfo",
                                                                                  title: "Utopia.SourceGenerator.Info",
                                                                                  messageFormat: msg,
                                                                                  category: "CodeGenerator",
                                                                                  DiagnosticSeverity.Info,
                                                                                  isEnabledByDefault: true);
        context.ReportDiagnostic(Diagnostic.Create(d, Location.None, args));
    }

    public static void ReportWarning(this GeneratorExecutionContext context, string msg, params object[] args)
    {
        var d = new DiagnosticDescriptor(id: "UtopiaSourceGeneratorWarning",
                                                                                  title: "Utopia.SourceGenerator.Warning",
                                                                                  messageFormat: msg,
                                                                                  category: "CodeGenerator",
                                                                                  DiagnosticSeverity.Warning,
                                                                                  isEnabledByDefault: true);
        context.ReportDiagnostic(Diagnostic.Create(d, Location.None, args));
    }
    public static string? GetProjectDir(this GeneratorExecutionContext context)
    {
        return context.AnalyzerConfigOptions.GlobalOptions
            .TryGetValue("build_property.projectdir", out var result) ? result : null;
    }

    public static IDictionary<string,AdditionalText> GenerateFileSystem(this GeneratorExecutionContext context)
    {
        var dict = new Dictionary<string,AdditionalText>();
        foreach(var file in context.AdditionalFiles)
        {
            dict.Add(
                    Path.GetFullPath(file.Path),
                    file);
            context.ReportWarning("find additional file {0}",Path.GetFullPath(file.Path));
        }
        return dict;
    }

    public static string? TryReadFile(this IDictionary<string, AdditionalText> fileSystem,string filePath)
    {
        if(fileSystem.TryGetValue(Path.GetFullPath(filePath), out var result)) {
            return result.GetText()!.ToString();
        }
        return null;
        
    }

}
