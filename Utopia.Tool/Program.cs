// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Text;
using McMaster.Extensions.CommandLineUtils;
using NLog;
using SemanticVersioning;
using Utopia.Tool.Generators;
using Version = SemanticVersioning.Version;

namespace Utopia.Tool;

public class Program
{

    private static readonly Lazy<Logger> s_loggerHandler = new(() => { return LogManager.GetLogger(typeof(Program).FullName); });

    private const string Art =
        """
                                      .-'''-.                                          
                             '   _    \                                        
                           /   /` '.   \_________   _...._      .--.           
                          .   |     \  '\        |.'      '-.   |__|           
                       .| |   '      |  '\        .'```'.    '. .--.           
                     .' |_\    \     / /  \      |       \     \|  |    __     
           _    _  .'     |`.   ` ..' /    |     |        |    ||  | .:--.'.   
          | '  / |'--.  .-'   '-...-'`     |      \      /    . |  |/ |   \ |  
         .' | .' |   |  |                  |     |\`'-.-'   .'  |  |`" __ | |  
         /  | /  |   |  |                  |     | '-....-'`    |__| .'.''| |  
        |   `'.  |   |  '.'               .'     '.                 / /   | |_ 
        '   .'|  '/  |   /              '-----------'               \ \._,\ '/ 
         `-'  `--'   `'-'                                            `--'  `"  
        """;

    /// <summary>
    /// Print logo. It will check the console weight to decide whether print it or no.
    /// </summary>
    public static void PrintArt()
    {
        try
        {
            string[] lines = Art.Split(new string[] { "\n", "\r", "\r\n" }, StringSplitOptions.None);

            int maxWeidth = lines.MaxBy((s) => s.Length)?.Length ?? int.MaxValue;

            if (Console.BufferWidth >= maxWeidth)
            {
                Console.WriteLine(Art);
            }
        }
        catch (NotImplementedException)
        {
            // avoid that Console.BufferWidth throw this
            return;
        }
    }

    public static bool DetectBatch()
    {
        // obviously,we're under CI environment
        if (Environment.GetEnvironmentVariable("CI") != null)
        {
            return true;
        }

        // may be a good way
        return Console.IsErrorRedirected || Console.IsOutputRedirected || Console.IsInputRedirected;
    }

    public static Version GetVersion()
    {
        return VersionUtility.UtopiaCoreVersion;
    }

    public static string GetProgramName()
    {
        return typeof(Program).Assembly.GetName().Name ?? "Utopia.Tool";
    }

    private static Logger _Logger => s_loggerHandler.Value;

    private static int Main(string[] args)
    {
        if (DetectBatch())
        {
            Core.Logging.LogManager.Init(Core.Logging.LogManager.LogOption.CreateBatch());
        }
        else
        {
            Core.Logging.LogManager.Init(Core.Logging.LogManager.LogOption.CreateDefault());
        }

        var sb = new StringBuilder("[ \"Utopia.Tools");

        foreach (string arg in args)
        {
            _ = sb.Append("\",\"");
            _ = sb.Append(arg.Replace("\"", "\\\""));
        }
        _ = sb.Append("\" ]");

        Console.WriteLine("args: {0}", sb.ToString());

        var app = new CommandLineApplication
        {
            Name = GetProgramName(),
            Description = "A C# tool which is used for utopia(and its plugin) development",
        };

        // build command
        _ = app.Command("extract-translation", TranslationFinder.Command);
        _ = app.Command("docs", GenerateDocs.Command);
        _ = app.Command("generate", GenerationCommand.Command);

        // build option
        _ = app.HelpOption(inherited: true);

        CommandOption version = app.Option("-v|--version", "show version", CommandOptionType.NoValue);
        CommandOption<bool> batch = app.Option<bool>(
            "--batch-mode",
            "enable/or disable batch mode. The default value was detected automatically.(e.g. false in CI, true in terminals)",
            CommandOptionType.SingleValue);
        batch.DefaultValue = DetectBatch();

        app.OnExecute(() =>
        {
            // detect batch
            if (batch.ParsedValue)
            {
                Core.Logging.LogManager.Init(Core.Logging.LogManager.LogOption.CreateBatch());
            }
            else
            {
                Core.Logging.LogManager.Init(Core.Logging.LogManager.LogOption.CreateDefault());
            }

            // detect version
            if (!version.HasValue())
            {
                return;
            }
            string v = GetVersion().ToString();
            if (v == null)
            {
                Console.WriteLine("Failed to get the Assembly version(null). return Default Value.");
            }
            Console.WriteLine("Licensed by AGPL 3.0-or-later");
            Console.WriteLine("Copyright 2020-2023 mingmoe(http://kawayi.moe)");
            PrintArt();
            return;
        });

        return app.Execute(args);
    }
}
