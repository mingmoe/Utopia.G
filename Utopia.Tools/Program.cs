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
using Utopia.Tools;

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
