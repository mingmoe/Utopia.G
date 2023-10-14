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
using Utopia.Tools.Generators;

namespace Utopia.Tools;

public class Program
{

    private static readonly Lazy<Logger> _loggerHandler = new(() => { return LogManager.GetLogger(typeof(Program).FullName); });

    private static Logger _Logger
    {
        get { return _loggerHandler.Value; }
    }

    static int Main(string[] args)
    {
        Core.Logging.LogManager.Init(true);

        var sb = new StringBuilder("[ \"Utopia.Tools");

        foreach(var arg in args)
        {
            sb.Append("\",\"");
            sb.Append(arg);
        }
        sb.Append("\" ]");

        _Logger.Info("arguments: {args}", sb.ToString());

        var app = new CommandLineApplication
        {
            Name = "Utopia.Tools",
            Description = "A C# tool which is used for utopia(and its plugin) development",
        };

        MSBuildLocator.RegisterDefaults();

        //-------------
        // 获取翻译条目
        //-------------
        app.Command("extractTranslate", TranslateFinder.Command);
        app.Command("docs", GenerateDocs.Command);
        app.Command("generate", GeneratorCommand.Command);

        app.HelpOption(inherited: true);

        return app.Execute(args);
    }
}
