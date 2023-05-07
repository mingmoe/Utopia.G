using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utopia.Core;
using Utopia.Core.Translate;

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

    private async Task _WalkDocument(Document file, Compilation compilation)
    {
        // read source file and create semantic model
        if (!file.TryGetText(out var text))
        {
            throw new InvalidOperationException($"failed to get the source of file {file.FilePath}");
        }

        SyntaxTree? tree = await file.GetSyntaxTreeAsync();

        if (tree == null)
        {
            _Logger.Info("skip file {file} because there is no syntax tree", file.FilePath);
            return;
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
                    return;
                }

                // check name
                var id = translate[0].GetText().ToString().Trim('"');
                var comment = translate[1].GetText().ToString().Trim('"');
                string? provider = null;

                // parse
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

                _Logger.Info("found translate item at {file}:{provider}->{translate} --{comment} ", file.FilePath, provider, id, comment);
            }
            else
            {
                _Logger.Trace("except name {expect} but found {name} at {file} {line}",
                    typeof(TranslateKey).FullName, symName,
                    file.FilePath, node.Span);
            }
        }
    }

    public async Task FindTranslateItem(Project project)
    {
        await project.GetCompilationAsync();
        if (!project.TryGetCompilation(out Compilation? compilation))
        {
            _Logger.Error("failed to compile project {project}", project.Name);
            return;
        }

        foreach (var file in project.Documents)
        {
            await this._WalkDocument(file, compilation);
        }
    }

}
