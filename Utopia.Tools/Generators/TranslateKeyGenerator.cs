// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Text;
using System.Xml.Serialization;
using NLog;
using Tomlyn;
using Utopia.Core.Transition;
using Utopia.Core.Utilities;

namespace Utopia.Tools.Generators;
public class TransitionConfiguration
{
    [XmlElement]
    public string TargetNamespace { get; set; } = "global";

    [XmlElement]
    public string TargetClass { get; set; } = "TranslationKeys";
}

public class TranslateKeyGenerator : IGenerator
{

    private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

    public const string TranslationClassName = "TranslationKeys";

    public string SubcommandName => "translate-key";

    /// <summary>
    /// Generate translation for file
    /// </summary>
    private static string _GenerateFor(string source, Dictionary<Guuid, TomlTranslateHumanItem> items
        , GeneratorOption option)
    {
        CsBuilder builder = new(null,source);

        builder.Using.Add("Utopia.Core.Transition");
        builder.Namespace = option.Configuration.TransitionConfiguration.TargetNamespace;

        builder.EmitClass(TranslationClassName, isPartial: true, isStatic: true, isPublic: true);

        foreach (KeyValuePair<Guuid, TomlTranslateHumanItem> item in items)
        {
            builder.EmitField("public", option.Configuration.TransitionConfiguration.TargetClass,
                item.Key.ToCsIdentifier(),
                defaultValue: $"TranslateKey.Create(\"{item.Key}\",\"{item.Value.Comment}\",\"{item.Value.Provider}\")", isReadonly: true, isStatic: true);
        }

        builder.CloseCodeBlock();

        return builder.Generate();
    }

    public void Execute(GeneratorOption option)
    {
        // read all translation files in translate-root directory
        string[] files = Directory.GetFiles(option.TargetProject.TranslationDirectory);

        // process
        TomlModelOptions tomlOpt = Guuid.AddTomlOption();
        IPluginDevFileSystem fs = option.TargetProject;

        foreach (string file in files)
        {
            try
            {
                Dictionary<Guuid, TomlTranslateHumanItem> item = Toml.ToModel<Dictionary<Guuid, TomlTranslateHumanItem>>(
                    File.ReadAllText(file, Encoding.UTF8), options: tomlOpt);

                string output = fs.GetGeneratedCsFilePath(file);

                _ = Directory.CreateDirectory(Path.GetDirectoryName(output)!);

                string generated = _GenerateFor(file, item, option);

                File.WriteAllText(output, generated, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "get an error when process file {file}", file);
                throw;
            }
        }
    }
}
