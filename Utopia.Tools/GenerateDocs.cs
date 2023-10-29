// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using Markdig;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.DocAsCode;
using Microsoft.DocAsCode.Dotnet;

namespace Utopia.Tools;

/// <summary>
/// 生成文档
/// </summary>
public class GenerateDocs
{

    public static void Command(CommandLineApplication configCmd)
    {
        CommandOption<string> configOpt = configCmd.Option<string>("-c|--config", "the config file", CommandOptionType.SingleValue);

        configOpt.DefaultValue = "docfx.json";

        configCmd.OnExecute(() =>
        {
            var options = new BuildOptions
            {
                // Enable custom markdown extensions here
                ConfigureMarkdig = pipeline => pipeline.UseCitations(),
            };

            DotnetApiCatalog.GenerateManagedReferenceYamlFiles(configOpt.Value()).Wait();
            Docset.Build(configOpt.Value(), options).Wait();
        });
    }

}
