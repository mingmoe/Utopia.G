// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using McMaster.Extensions.CommandLineUtils;

namespace Utopia.Tools.Generators;
public class GeneratorCommand
{

    public static void Command(CommandLineApplication application)
    {
        CommandOption version = application.Option("--version-file <FILE>", "the path of version file", CommandOptionType.SingleValue);
        CommandOption pluginInfo = application.Option("--plugin-info-file <FILE>", "the path of plugin information file", CommandOptionType.SingleValue);
        CommandOption translateDir = application.Option("--translate-dir <DIR>", "the path to the translate files", CommandOptionType.SingleValue);
        CommandOption assertsDir = application.Option("--assert-dir <DIR>", "the path to the assert files", CommandOptionType.SingleValue);
        CommandOption generatedDir = application.Option("--generated-dir <DIR>", "the path to the generated .cs files", CommandOptionType.SingleValue);
        CommandOption project = application.Option("--project <DIR>", "the path to the projects which need to generate source files", CommandOptionType.SingleValue).IsRequired();
        CommandOption @namespace = application.Option("--namespace <NAMESPACE>", "the target namespace of the generated code", CommandOptionType.SingleValue);

        @namespace.DefaultValue = "global";

        IPluginDevFileSystem createFileSystem(string project)
        {
            PluginDevFileSystem system = new(project);
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
                system.TranslationDirectory = translateDir.Value()!;
            }
            if (assertsDir.HasValue())
            {
                system.AssetsDirectory = assertsDir.Value()!;
            }
            if (generatedDir.HasValue())
            {
                system.GeneratedDirectory = generatedDir.Value()!;
            }

            ((IPluginDevFileSystem)system).CreateNotExistsDirectory();

            return system;
        }

        GeneratorOption getOption() => new(@namespace.Value()!, createFileSystem(project.Value()!));

        var generators = new IGenerator[] {
            new PluginInformationGenerator(),
            new PluginGenerator(),
            new TranslateKeyGenerator(),
        };

        foreach (IGenerator generator in generators)
        {
            _ = application.Command(generator.SubcommandName, (config) =>
            {
                generator.Command(config, getOption);
            });
        }

    }
}
