#region copyright
// This file(may named GenerateDocs.cs) is a part of the project: Utopia.Analyzer.
// 
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// 
// This file is part of Utopia.Analyzer.
//
// Utopia.Analyzer is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Utopia.Analyzer is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with Utopia.Analyzer. If not, see <https://www.gnu.org/licenses/>.
#endregion

using Markdig;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.DocAsCode;
using Microsoft.DocAsCode.Dotnet;

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
