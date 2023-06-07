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

        //-------------
        // 获取翻译条目
        //-------------
        app.Command("getTranslateItem", TranslateFinder.Command);
        app.Command("generateDocuments", GenerateDocs.Command);

        app.HelpOption(inherited: true);

        return app.Execute(args);
    }
}
