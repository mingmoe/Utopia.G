using Markdig;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Build.Evaluation;
using Microsoft.DocAsCode;
using Microsoft.DocAsCode.Dotnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utopia.Analyzer;

/// <summary>
/// 生成文档
/// </summary>
public class GenerateDocs
{

    public static void Command(CommandLineApplication configCmd)
    {
        var configOpt = configCmd.Option<string>("-c|--config", "the config file", CommandOptionType.SingleValue);

        configOpt.DefaultValue = "docfx.json";

        configCmd.OnExecute(() =>
        {
            var options = new BuildOptions
            {
                // Enable custom markdown extensions here
                ConfigureMarkdig = pipeline => pipeline.UseCitations(),
            };

            DotnetApiCatalog.GenerateManagedReferenceYamlFiles(configOpt.Value()).Wait();
            Microsoft.DocAsCode.Docset.Build(configOpt.Value(), options).Wait();
        });
    }

}
