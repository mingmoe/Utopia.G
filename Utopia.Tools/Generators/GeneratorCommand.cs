// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Configuration;
using System.Xml.Serialization;
using Json.Schema;
using McMaster.Extensions.CommandLineUtils;
using Utopia.Tools.Generators.Server;

namespace Utopia.Tools.Generators;
public class GeneratorCommand
{
    public static void Command(CommandLineApplication application)
    {
        CommandOption config = application.Option(
            "--configuration <FILE>",
            "the path to the configuration file",
            CommandOptionType.SingleValue)
            .IsRequired(errorMessage: "you must provide a configuration file");
        CommandOption project = application.Option(
            "--project <DIR>",
            "the path to the root of the project which need to generate source files",
            CommandOptionType.SingleValue)
            .IsRequired(errorMessage: "you must provide a project!");

        (IPluginDevFileSystem, Configuration) createFileSystem(string project)
        {
            PluginDevFileSystem system = new(project);

            XmlSerializer serializer =
                new(typeof(Configuration));

            Configuration configuration;

            using (Stream reader = new FileStream(config.Value()! , FileMode.Open))
            {
                configuration = (Configuration)(serializer.Deserialize(reader) ?? throw new InvalidCastException("The "));
            }

            configuration.ApplyToFileSystem(system);

            ((IPluginDevFileSystem)system).CreateNotExistsDirectory();
            return (system,configuration);
        }

        application.OnExecute(() =>
        {
            (IPluginDevFileSystem system, Configuration configuration) = createFileSystem(project.Value()!);
            GeneratorOption option = new(configuration, system);

            Dictionary<string, IGenerator> generators = new();
            foreach (IGenerator generator in (IGenerator[])[
                new PluginInformationGenerator(),
                new PluginGenerator(),
                new ServerEntityGenerator(),
                new TranslateKeyGenerator()])
            {
                generators.Add(generator.SubcommandName, generator);
            }

            foreach (string generator in configuration.Generators)
            {
                generators[generator].Execute(option);
            }
        });
    }
}
