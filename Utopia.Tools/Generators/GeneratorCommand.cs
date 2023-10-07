using McMaster.Extensions.CommandLineUtils;

namespace Utopia.Tools.Generators;
public class GeneratorCommand
{
    public static void Command(CommandLineApplication application)
    {
        var version = application.Option("--version-file <FILE>", "the path of version file", CommandOptionType.SingleValue);
        var pluginInfo = application.Option("--plugin-info-file <FILE>", "the path of plugin information file", CommandOptionType.SingleValue);
        var translateDir = application.Option("--translate-dir <DIR>", "the path to the translate files", CommandOptionType.SingleValue);
        var assertsDir = application.Option("--assert-dir <DIR>", "the path to the assert files", CommandOptionType.SingleValue);
        var generatedDir = application.Option("--generated-dir <DIR>", "the path to the generated .cs files", CommandOptionType.SingleValue);
        var project = application.Option("--project <DIR>", "the path to the projects which need to generate source files", CommandOptionType.SingleValue).IsRequired();
        var @namespace = application.Option("--namespace <NAMESPACE>", "the target namespace of the generated code", CommandOptionType.SingleValue).IsRequired();

        IFileSystem createFileSystem(string project)
        {
            FileSystem system = new(project);
            if (version.HasValue())
            {
                system.VersionFile = version.Value()!;
            }
            if (pluginInfo.HasValue())
            {
                system.PluginInfoFile = pluginInfo.Value()!;
            }
            if (translateDir.HasValue())
            {
                system.TranslateDir = translateDir.Value()!;
            }
            if (assertsDir.HasValue())
            {
                system.AssertDir = assertsDir.Value()!;
            }
            if (generatedDir.HasValue())
            {
                system.GeneratedDir = generatedDir.Value()!;
            }

            ((IFileSystem)system).CreateNotExistsDirectory();

            return system;
        }

        application.Command("pluginInfo", (sub) =>
        {
            sub.OnExecute(() =>
            {
                var generator = new PluginInformationGenerator();

                var option = new GeneratorOption(@namespace.Value()!, createFileSystem(project.Value()!));

                generator.Execute(option);
            });
        });
    }
}
