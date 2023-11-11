// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Configuration;
using System.Xml.Schema;
using System.Xml.Serialization;
using Json.Schema;
using McMaster.Extensions.CommandLineUtils;
using NLog;
using NLog.Fluent;
using Utopia.Tools.Generators.Server;
using XmlSchemaClassGenerator;

namespace Utopia.Tools.Generators;

public static class GenerationCommand
{
    private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

    private static void _GenerateXmlSchema(string path)
    {
        try
        {
            XmlSchemas schemas = new();
            XmlSchemaExporter exporter = new(schemas);

            XmlTypeMapping mapping = new XmlReflectionImporter().ImportTypeMapping(typeof(Configuration));
            exporter.ExportTypeMapping(mapping);

            using (FileStream writer = File.OpenWrite(path))
            {
                foreach (XmlSchema schema in schemas)
                {
                    schema.Write(writer);
                }
            }

            s_logger.Info("XSD generated successfully");
        }
        catch (Exception ex)
        {
            s_logger.Info(ex,"Error in generating XSD. Error");
        }
    }

    private static Configuration _LoadConfiguration(string configFile)
    {
        XmlSerializer serializer =
            new(typeof(Configuration));

        Configuration configuration;

        using (Stream reader = File.OpenRead(configFile))
        {
            configuration = (Configuration)(serializer.Deserialize(reader) ?? throw new InvalidCastException("The XmlSerializer.Deserialize return null"));
        }

        return configuration;
    }

    private static IPluginDevFileSystem _GetFileSystem(SubprojectConfiguration configuration)
    {
        var filesystem = new PluginDevFileSystem(configuration.Path);

        configuration.Configuration.ApplyToFileSystem(filesystem);

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
            "change the working directory when process projects(change to the configuration file path or subproject path)",
            CommandOptionType.SingleValue);

        changeDirectory.DefaultValue = true.ToString().ToLower();

        application.OnExecute(() =>
        {
            // register generators
            Dictionary<string, IGenerator> generators = new();
            foreach (IGenerator generator in (IGenerator[])[
                new PluginInformationGenerator(),
                new PluginGenerator(),
                new ServerEntityGenerator(),
                new TranslateKeyGenerator()])
            {
                generators.Add(generator.SubcommandName, generator);
            }

            // set up change directory method
            var changeDir = bool.Parse(changeDirectory.Value()!);

            void cd(string path)
            {
                path = Path.GetFullPath(path);
                if (changeDir)
                {
                    s_logger.Info("change working directory to:{directory}", path);
                    Environment.CurrentDirectory = path;
                }
                s_logger.Info("process under directory:{directory}",path);
            }

            // read configurations
            var configurationFilePath = Path.GetFullPath(config.Value()!);

            cd(Path.GetDirectoryName(configurationFilePath) ?? ".");
            var configuration = _LoadConfiguration(configurationFilePath);

            // generate xml schema
            if (configuration.GenerateXmlSchemaFileTo != null)
            {
                _GenerateXmlSchema(configuration.GenerateXmlSchemaFileTo);
            }

            // process subprojects
            foreach (var subproject in configuration.Subprojects)
            {
                cd(subproject.Path);

                var option = new GeneratorOption(configuration,subproject, _GetFileSystem(subproject));

                foreach(var generator in subproject.Generators)
                {
                    generators[generator].Execute(option);
                }
            }
        });
    }
}
