using McMaster.Extensions.CommandLineUtils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utopia.Core;
using Utopia.Core.Translate;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Utopia.Analyzer;

/// <summary>
/// 翻译寻找器
/// </summary>
public class TranslateFinder
{

    private static readonly Lazy<Logger> _logger = new(() => { return LogManager.GetLogger(typeof(TranslateFinder).FullName); });

    private static Logger _Logger
    {
        get
        {
            return _logger.Value;
        }
    }

    public record Item(in string SlnFilePath, in string ProjectGuid, in string SourceFilePath, in string SourceSpan,
        in string TranslateGuuid,
        in string? TranslateProviderGuuid,
        in string TranslateComment)
    {
    }

    private static async Task<Item[]> _WalkDocument(Document file, Compilation compilation)
    {
        ArgumentNullException.ThrowIfNull(nameof(file));
        ArgumentNullException.ThrowIfNull(nameof(compilation));

        List<Item> items = new();
        // read source file and create semantic model
        if (!file.TryGetText(out var text))
        {
            throw new InvalidOperationException($"failed to get the source of file {file.FilePath}");
        }

        SyntaxTree? tree = await file.GetSyntaxTreeAsync();

        if (tree == null)
        {
            _Logger.Info("skip file {file} because there is no syntax tree", file.FilePath);
            return Array.Empty<Item>();
        }

        CompilationUnitSyntax root = tree.GetCompilationUnitRoot();

        SemanticModel model = compilation.GetSemanticModel(tree);

        // find syntax
        var nodes = root.DescendantNodes().OfType<ObjectCreationExpressionSyntax>();

        // look up the name we want
        foreach (var node in nodes)
        {
            var sym = model.GetSymbolInfo(node);
            var symName = sym.Symbol?.ContainingType.ToString();

            // check name
            if (symName == typeof(TranslateKey).FullName)
            {
                // find strings
                var translate = node.DescendantNodes().OfType<LiteralExpressionSyntax>().ToArray();

                if (translate.Length < 2)
                {
                    _Logger.Error("the syntax is illegal(only support literal):at {file} {span}", file.FilePath, node.Span);
                    return Array.Empty<Item>();
                }

                // check name
                var id = translate[0].GetText().ToString().Trim('"');
                var comment = translate[1].GetText().ToString().Trim('"');
                string? provider = null;

                // parse,there are two or three arguments we need to think
                if (translate.Length > 2)
                {
                    provider = translate[2].GetText().ToString().Trim('"');
                    if (!Guuid.CheckGuuid(provider))
                    {
                        _Logger.Error("the translate provider(guuid) is illegal:at {file} {span}", file.FilePath, node.Span);
                    }
                }
                if (!Guuid.CheckGuuid(id))
                {
                    _Logger.Error("the translate id(guuid) is illegal:at {file} {span}", file.FilePath, node.Span);
                }

                items.Add(new Item(
                    file.Project.Solution.FilePath ?? "UNKNOWN",
                    file.Project.Id.Id.ToString() ?? "UNKNOWN",
                    file.FilePath ?? "UNKNOWN",
                    node.Span.ToString(),
                    id,
                    provider,
                    comment));
                _Logger.Info("found translate item at {file}:{provider}->{translate} --{comment} ",
                    file.FilePath, provider, id, comment);
            }
            else
            {
                _Logger.Debug("except name {expect} but found {name} at {file} {line}",
                    typeof(TranslateKey).FullName, symName,
                    file.FilePath, node.Span);
            }
        }

        return items.ToArray();
    }

    public static async Task<Item[]> FindTranslateItem(Project project)
    {
        if (!project.TryGetCompilation(out Compilation? compilation))
        {
            return Array.Empty<Item>();
        }

        List<Item> items = new();

        foreach (var file in project.Documents)
        {
            items.AddRange(await _WalkDocument(file, compilation));
        }

        return items.ToArray();
    }

    public static void Command(CommandLineApplication configCmd)
    {
        var slnOpt = configCmd.Option<string>("-s|--sln", "the path to the .sln file", CommandOptionType.SingleValue).IsRequired();
        var projOpt = configCmd.Option<string>
        ("-p|--project", "the project guid of the sln that you want to get translate item", CommandOptionType.SingleValue);
        var optOpt = configCmd.Option<string>("-o|--output", "the output file", CommandOptionType.SingleValue);

        projOpt.DefaultValue = null;
        var defaultOpt = "./translate-items.json";
        optOpt.DefaultValue = defaultOpt;

        configCmd.OnExecute(async () =>
        {
            var sln = slnOpt.Value();
            var proj = projOpt.Value();
            var opt = optOpt.Value();

            _Logger.Info("load solution {sln}", sln);

            var projs = Utility.OpenSlnToProject(sln!, proj);
            _ = await Utility.GetCompilation(projs);

            var finder = new TranslateFinder();

            List<Task<Item[]>> tasks = new();
            foreach (var item in projs)
            {
                tasks.Add(FindTranslateItem(item));
            }
            Task.WaitAll(tasks.ToArray());
            var results = tasks.Select((t) => { return t.Result; }).Aggregate((old, n) =>
            {
                var ret = new Item[old.Length + n.Length];
                Array.Copy(old, ret, old.Length);
                Array.Copy(n, ret[old.Length..], n.Length);
                return ret;
            });

            List<Item> items = new(results);
            File.WriteAllText(opt!, JsonSerializer.Serialize(items), Encoding.UTF8);
        });

    }
}
