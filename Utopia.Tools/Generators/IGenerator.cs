// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using McMaster.Extensions.CommandLineUtils;

namespace Utopia.Tools.Generators;

/// <summary>
/// The same from microsoft is shit. So use this.
/// </summary>
public interface IGenerator
{
    string SubcommandName { get; }

    void Command(CommandLineApplication application, Func<GeneratorOption> option)
    {
        NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        application.OnExecute(() =>
        {
            using GeneratorOption opt = option.Invoke();

            logger.Info("Generate source for:\n{}", opt);

            Execute(opt);
        });
    }

    /// <summary>
    /// Note: this may be call many times
    /// </summary>
    void Execute(GeneratorOption option);
}
