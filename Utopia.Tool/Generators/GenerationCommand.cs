// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Configuration;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Json.Schema;
using McMaster.Extensions.CommandLineUtils;
using NLog;
using NLog.Fluent;
using Utopia.Core;
using Utopia.Tool;

namespace Utopia.Tool.Generators;

public static class GenerationCommand
{
    private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

    private static void _GenerateXmlSchema(string path)
    {
        try
        {
            var schemas = Xml.GetXmlSchema<Configuration>();

            using (FileStream writer = File.Open(path, FileMode.OpenOrCreate | FileMode.Truncate))
            {
                Xml.WriteXmlSchemas(schemas, writer);
            }

            s_logger.Info("XSD generated successfully");
        }
        catch (Exception ex)
        {
            s_logger.Info(ex, "Error in generating XSD.");
        }
    }

    private static Configuration _LoadConfiguration(string configFile)
    {
        XmlSerializer serializer =
            new(typeof(Configuration));

        Configuration configuration;

        using (Stream reader = File.OpenRead(configFile))
        {
            configuration = (Configuration)(serializer.Deserialize(reader) ?? throw new XmlException("The XmlSerializer.Deserialize return null"));
        }

        if (configuration.RootDirectory == null)
        {
            configuration.RootDirectory = Path.GetDirectoryName(Path.GetFullPath(configFile));
        }

        configuration.VersionFile = Path.GetFullPath(configuration.VersionFile, configuration.RootDirectory!);

        return configuration;
    }

    private static IPluginDevFileSystem _GetFileSystem(
        string root,
        SubprojectConfiguration configuration,
        string backupVersionFile)
    {
        var filesystem = new PluginDevFileSystem(Path.GetFullPath(configuration.Path, root));

        configuration.Configuration.ApplyToFileSystem(filesystem, backupVersionFile);

        return filesystem;
    }

    public static void Command(CommandLineApplication application)
    {
        application.Description = "this subcommand will generate something useful(like .cs/.xml and so on) for the utopia plugin";
        CommandOption config = application.Option<string>(
            "--configuration <FILE>",
            "the path to the configuration file",
            CommandOptionType.SingleValue)
            .IsRequired(errorMessage: "you must provide a configuration file");
        CommandOption project = application.Option<string>(
            "--project <DIR>",
            "the path to the root of the project which need to generate source files",
            CommandOptionType.SingleValue)
            .IsRequired(errorMessage: "you must provide a project!");
        CommandOption changeDirectory = application.Option<bool>(
            "--change-directory-to",
            "change the working directory to the one where the configuration file is",
            CommandOptionType.SingleValue);

        changeDirectory.DefaultValue = true.ToString().ToLower();

        application.OnExecute(() =>
        {
            // register generators
            Dictionary<string, IGenerator> generators = new();
            foreach (IGenerator generator in (IGenerator[])[
                new PluginInformationGenerator(),
                new Server.PluginGenerator(),
                new EntityGenerator(),
                new TranslateKeyGenerator()])
            {
                generators.Add(generator.SubcommandName, generator);
            }

            // set up change directory method
            var changeDir = bool.Parse(changeDirectory.Value()!);

            // read configurations and change directory
            var configurationFilePath = Path.GetFullPath(config.Value()!);

            var configuration = _LoadConfiguration(configurationFilePath);

            void cd(string path)
            {
                path = Path.GetFullPath(path, configuration.RootDirectory!);
                if (changeDir)
                {
                    s_logger.Info("change working directory to:{directory}", path);
                    Environment.CurrentDirectory = path;
                }
                s_logger.Info("process under directory:{directory}", path);
            }

            cd(".");

            // generate xml schema
            if (configuration.GenerateXmlSchemaFileTo != null)
            {
                _GenerateXmlSchema(configuration.GenerateXmlSchemaFileTo);
            }

            // get an translation manager
            var translationManager = new TranslateManager();

            // process subprojects
            foreach (var subproject in configuration.Subprojects)
            {
                cd(subproject.Path);

                var option = new GeneratorOption(
                    configuration,
                    subproject,
                    _GetFileSystem(configuration.RootDirectory!, subproject, configuration.VersionFile),
                    translationManager);

                option.CurrentFileSystem.CreateNotExistsDirectory();

                foreach (var generator in subproject.Generators)
                {
                    generators[generator].Execute(option);
                }
            }
        });
    }
}
