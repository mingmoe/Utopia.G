using McMaster.Extensions.CommandLineUtils;

namespace Utopia.Tools.Generators;

/// <summary>
/// The same from microsoft is shit. So use this.
/// </summary>
public interface IGenerator
{
    string SubcommandName { get; }

    void Command(CommandLineApplication application,Func<GeneratorOption> option)
    {
        var logger = NLog.LogManager.GetCurrentClassLogger();
        application.OnExecute(() =>
        {
            using var opt = option.Invoke();

            logger.Info("Generate source for:\n{}", option);

            this.Execute(opt);
        });
    }

    /// <summary>
    /// Note: this may be call many times
    /// </summary>
    void Execute(GeneratorOption option);
}
