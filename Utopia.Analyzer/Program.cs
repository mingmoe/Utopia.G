using Castle.Core.Logging;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Text;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Utopia.Analyzer;
using Utopia.Core;

namespace Utopia.Analysis;

public class Program
{

    private static readonly Lazy<Logger> _loggerHandler = new(() => { return LogManager.GetLogger(typeof(Program).FullName); });

    private static Logger _Logger
    {
        get { return _loggerHandler.Value; }
    }

    static int Main(string[] args)
    {
        Utopia.Core.Logging.LogManager.Init(true);

        _Logger.Info("logging system started");
        _Logger.Info("arguments: {args}", args);

        var app = new CommandLineApplication
        {
            Name = "Utopia.Analyzer",
            Description = "A C# tool is used for utopia(and its plugin) development",
        };

        MSBuildLocator.RegisterDefaults();

        // 获取翻译条目
        app.Command("getTranslateItem", configCmd =>
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

                var msWorkspace = MSBuildWorkspace.Create();
                var t = msWorkspace.OpenSolutionAsync(sln!);
                t.Wait();
                var solution = t.Result;

                var finder = new TranslateFinder();

                Project[] projs = proj == null ? solution.Projects.ToArray() : new Project[1] { solution.GetProject(ProjectId.CreateFromSerialized(Guid.Parse(proj)))
                ?? throw new ArgumentException($"the project guuid {proj} not found") };

                List<Task<TranslateFinder.Item[]>> tasks = new();
                foreach (var item in projs)
                {
                    tasks.Add(TranslateFinder.FindTranslateItem(item));
                }
                foreach (var task in tasks)
                {
                    await task;
                }

                List<TranslateFinder.Item> items = new();
                foreach (var task in tasks)
                {
                    items.AddRange(task.Result);
                }
                File.WriteAllText(opt ?? defaultOpt, JsonSerializer.Serialize(items), Encoding.UTF8);

            });
        });

        app.HelpOption(inherited: true);

        return app.Execute(args);
    }
}
