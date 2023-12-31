// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Diagnostics;
using System.Text;
using NLog;
using NLog.Layouts;
using NLog.Targets;

namespace Utopia.Core.Logging;

/// <summary>
/// 日志管理器
/// </summary>
public static class LogManager
{

    /// <summary>
    /// log option
    /// </summary>
    public class LogOption
    {
        public bool EnableConsoleOutput { get; set; } = true;

        public bool ColorfulOutput { get; set; }

        /// <summary>
        /// <see cref="ColorfulOutput"/> must be ture to enable this.
        /// </summary>
        public bool EnableDateRegexColor { get; set; }

        public bool EnableConsoleDebugOutput { get; set; }

        public LogOption() { }

        /// <summary>
        /// Create a new default log option. e.g. use colorful output.
        /// </summary>
        /// <returns></returns>
        public static LogOption CreateDefault() => new()
        {
            ColorfulOutput = true,
            EnableDateRegexColor = true,
            EnableConsoleDebugOutput = false,
        };

        /// <summary>
        /// Create a new log option for batch. e.g. disable colorful output.
        /// </summary>
        /// <returns></returns>
        public static LogOption CreateBatch() => new()
        {
            ColorfulOutput = false,
            EnableDateRegexColor = false,
            EnableConsoleDebugOutput = true,
        };
    }

    /// <summary>
    /// 初始化日志
    /// </summary>
    /// <param name="enableRegexColored">是否启用正则表达式进行着色，这会导致性能降低，但是输出将会变得beautiful</param>
    public static void Init(LogOption option)
    {
        var config = new NLog.Config.LoggingConfiguration();

        // config file
        var logfile = new FileTarget("logfile")
        {
            FileName = "Log/Current.log",
            LineEnding = LineEndingMode.LF,
            Encoding = Encoding.UTF8,
            ArchiveFileName = "Log/Archived.{###}.log",
            ArchiveNumbering = ArchiveNumberingMode.DateAndSequence,
            ArchiveDateFormat = "yyyy.MM.dd",
            MaxArchiveFiles = 128,
            ArchiveOldFileOnStartup = true,
            EnableArchiveFileCompression = true,
            ArchiveEvery = FileArchivePeriod.Day
        };

        // config console
        TargetWithLayoutHeaderAndFooter logconsole = null!;

        // detect color option
        if (option.ColorfulOutput)
        {
            var colorConsole = new ColoredConsoleTarget("logconsole")
            {
                UseDefaultRowHighlightingRules = false,
                Encoding = Encoding.UTF8,
                EnableAnsiOutput = true
            };
            colorConsole.RowHighlightingRules.Add(new ConsoleRowHighlightingRule()
            {
                Condition = "level == LogLevel.Debug",
                ForegroundColor = ConsoleOutputColor.Blue,
            });
            colorConsole.RowHighlightingRules.Add(new ConsoleRowHighlightingRule()
            {
                Condition = "level == LogLevel.Trace",
                ForegroundColor = ConsoleOutputColor.Cyan,
            });
            colorConsole.RowHighlightingRules.Add(new ConsoleRowHighlightingRule()
            {
                Condition = "level == LogLevel.Info",
                ForegroundColor = ConsoleOutputColor.White,
            });
            colorConsole.RowHighlightingRules.Add(new ConsoleRowHighlightingRule()
            {
                Condition = "level == LogLevel.Warn",
                ForegroundColor = ConsoleOutputColor.Yellow,
            });
            colorConsole.RowHighlightingRules.Add(new ConsoleRowHighlightingRule()
            {
                Condition = "level == LogLevel.Error",
                ForegroundColor = ConsoleOutputColor.Red,
            });
            colorConsole.RowHighlightingRules.Add(new ConsoleRowHighlightingRule()
            {
                Condition = "level == LogLevel.Fatal",
                ForegroundColor = ConsoleOutputColor.DarkRed,
            });
            logconsole = colorConsole;
        }
        else
        {
            var console = new ConsoleTarget("logconsole")
            {
                Encoding = Encoding.UTF8,
            };
            logconsole = console;
        }

        if (logconsole is ColoredConsoleTarget colored && option.EnableDateRegexColor)
        {
            // colored datetime
            colored.WordHighlightingRules.Add(new ConsoleWordHighlightingRule()
            {
                BackgroundColor = ConsoleOutputColor.Blue,
                ForegroundColor = ConsoleOutputColor.White,
                CompileRegex = true,
                Regex = @"\[\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{4}\]"
            });
        }

        // setup output file format
        logfile.Layout = new JsonLayout()
        {
            Attributes =
            {
                new JsonAttribute("time", "${longdate}"),
                new JsonAttribute("level", "${level}"),
                new JsonAttribute("thread","${threadname}"),
                new JsonAttribute("logger","${logger}"),
                new JsonAttribute("raw-message","${message:raw=true}"),
                new JsonAttribute("message", "${message}"),
                new JsonAttribute("properties", new JsonLayout { IncludeEventProperties = true, MaxRecursionLimit = 8 }, encode: false),
                new JsonAttribute("exception", new JsonLayout
                {
                    Attributes =
                    {
                        new JsonAttribute("callsite","${callsite}"),
                        new JsonAttribute("type", "${exception:format=type}"),
                        new JsonAttribute("message", "${exception:format=message}"),
                        new JsonAttribute("stacktrace", "${exception:format=tostring}"),
                    }
                },
                encode: false) // don't escape layout
            },
            IndentJson = true,
        };

        // 设置更好的异常格式
        _ = NLog.LogManager.Setup().SetupExtensions((e) =>
        {
            _ = e.RegisterLayoutRenderer("demystifiedException", (e) =>
            {
                if (e.Exception != null)
                {
                    e.Exception = e.Exception.Demystify();
                    return e.Exception.ToStringDemystified();
                }
                return string.Empty;
            });
        });
        logconsole.Layout = @"[${longdate}][${level}][${threadname}::${logger}]:${message}${onexception:inner=${newline}${demystifiedException}}";

        // set up
        if (option.EnableConsoleOutput && option.EnableConsoleDebugOutput)
        {
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logconsole);
        }
        else if(option.EnableConsoleOutput)
        {
            config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
        }
        config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

        NLog.LogManager.Configuration = config;
        NLog.LogManager.ReconfigExistingLoggers();
        NLog.LogManager.Flush();
    }

    /// <summary>
    /// 关闭日志
    /// </summary>
    public static void Shutdown() => NLog.LogManager.Shutdown();

}
