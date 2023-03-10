//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using NLog;
using NLog.Targets;
using System.Text;

namespace Utopia.Core;

/// <summary>
/// 日志管理器
/// </summary>
public static class LogManager
{

    /// <summary>
    /// 初始化日志
    /// </summary>
    /// <param name="enableRegexColored">是否启用正则表达式进行着色，这会导致性能降低，但是输出将会变得beautiful</param>
    public static void Init(bool enableRegexColored)
    {
        var config = new NLog.Config.LoggingConfiguration();

        // config file
        var logfile = new NLog.Targets.FileTarget("logfile")
        {
            FileName = "log/current.log",
            LineEnding = LineEndingMode.LF,
            Encoding = Encoding.UTF8,
            ArchiveFileName = "log/archived.{###}.log",
            ArchiveNumbering = ArchiveNumberingMode.DateAndSequence,
            ArchiveDateFormat = "yyyy.MM.dd",
            MaxArchiveFiles = 128,
            ArchiveOldFileOnStartup = true,
            EnableArchiveFileCompression = true,
            ArchiveEvery = FileArchivePeriod.Day
        };

        // config console
        var logconsole = new ColoredConsoleTarget("logconsole")
        {
            UseDefaultRowHighlightingRules = false,
            Encoding = Encoding.UTF8,
            EnableAnsiOutput = true
        };
        logconsole.RowHighlightingRules.Add(new ConsoleRowHighlightingRule()
        {
            Condition = "level == LogLevel.Debug",
            ForegroundColor = ConsoleOutputColor.Blue,
        });
        logconsole.RowHighlightingRules.Add(new ConsoleRowHighlightingRule()
        {
            Condition = "level == LogLevel.Trace",
            ForegroundColor = ConsoleOutputColor.Cyan,
        });
        logconsole.RowHighlightingRules.Add(new ConsoleRowHighlightingRule()
        {
            Condition = "level == LogLevel.Info",
            ForegroundColor = ConsoleOutputColor.White,
        });
        logconsole.RowHighlightingRules.Add(new ConsoleRowHighlightingRule()
        {
            Condition = "level == LogLevel.Warn",
            ForegroundColor = ConsoleOutputColor.Yellow,
        });
        logconsole.RowHighlightingRules.Add(new ConsoleRowHighlightingRule()
        {
            Condition = "level == LogLevel.Error",
            ForegroundColor = ConsoleOutputColor.Red,
        });
        logconsole.RowHighlightingRules.Add(new ConsoleRowHighlightingRule()
        {
            Condition = "level == LogLevel.Fatal",
            ForegroundColor = ConsoleOutputColor.DarkRed,
        });
        if (enableRegexColored)
        {
            // colored datetime
            logconsole.WordHighlightingRules.Add(new ConsoleWordHighlightingRule()
            {
                BackgroundColor = ConsoleOutputColor.Blue,
                ForegroundColor = ConsoleOutputColor.White,
                CompileRegex = true,
                Regex = @"\[\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{4}\]"
            });
        }

        // setup
        logfile.Layout = @"[${longdate}][${level}][${threadname}::${logger}]:${message}${onexception:inner=${newline}${exception}}";
        logconsole.Layout = logfile.Layout;

        config.AddRule(LogLevel.Debug, LogLevel.Fatal, logconsole);
        config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

        NLog.LogManager.Configuration = config;
        NLog.LogManager.Flush();
    }

    /// <summary>
    /// 关闭日志
    /// </summary>
    public static void Shutdown()
    {
        NLog.LogManager.Shutdown();
    }

}
