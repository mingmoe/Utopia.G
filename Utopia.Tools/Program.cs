#region copyright
// This file(may named Program.cs) is a part of the project: Utopia.Analyzer.
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

using McMaster.Extensions.CommandLineUtils;
using Microsoft.Build.Locator;
using NLog;
using System.Text;
using Utopia.Core.Logging;
using Utopia.Tools.Generators;

namespace Utopia.Tools;

public class Program
{

    private static readonly Lazy<Logger> _loggerHandler = new(() => { return NLog.LogManager.GetLogger(typeof(Program).FullName); });

    private const string _art =
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
            var lines = _art.Split(new string[] { "\n","\r","\r\n" }, StringSplitOptions.None);

            var maxWeidth = lines.MaxBy((s) => s.Length)?.Length ?? int.MaxValue;

            if(Console.BufferWidth >= maxWeidth)
            {
                Console.WriteLine(_art);
            }
        }
        catch(NotImplementedException)
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
        if (Console.IsErrorRedirected || Console.IsOutputRedirected || Console.IsInputRedirected)
        {
            return true;
        }

        return false;
    }

    private static Logger _Logger
    {
        get { return _loggerHandler.Value; }
    }

    static int Main(string[] args)
    {
        if(DetectBatch())
        {
            Core.Logging.LogManager.Init(Core.Logging.LogManager.LogOption.CreateBatch());
        }
        else
        {
            Core.Logging.LogManager.Init(Core.Logging.LogManager.LogOption.CreateDefault());
        }

        var sb = new StringBuilder("[ \"Utopia.Tools");

        foreach(var arg in args)
        {
            sb.Append("\",\"");
            sb.Append(arg);
        }
        sb.Append("\" ]");

        _Logger.Debug("arguments: {args}", sb.ToString());

        var app = new CommandLineApplication
        {
            Name = "Utopia.Tools",
            Description = "A C# tool which is used for utopia(and its plugin) development",
        };

        // build command
        app.Command("extractTranslate", TranslateFinder.Command);
        app.Command("docs", GenerateDocs.Command);
        app.Command("generate", GeneratorCommand.Command);

        // build option
        app.HelpOption(inherited: true);

        var version = app.Option("-v|--version", "show version", CommandOptionType.NoValue);
        var batch = app.Option<bool>(
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
            var v = typeof(Program).Assembly.GetName().Version?.ToString();
            Console.WriteLine(v ?? "0.0.0-FAIL-TO-GET");
            if (v == null)
            {
                Console.WriteLine("Failed to get the Assembly verion(null). return Default Value.");
            }
            Console.WriteLine("Licensed by AGPL 3.0-or-later");
            Console.WriteLine("Copyright 2020-2023 mingmoe(http://kawayi.moe)");
            PrintArt();
            return;
        });

        return app.Execute(args);
    }
}

