// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Text;
using System.Xml;
using System.Xml.Serialization;
using NLog;
using Utopia.Core.Transition;
using Utopia.Core.Utilities;

namespace Utopia.Tools.Generators;

public class TranslationConfiguration
{
    [XmlElement]
    public string TargetNamespace { get; set; } = "global";

    [XmlElement]
    public string TargetClass { get; set; } = "TranslationKeys";
}

public class TranslateKeyGenerator : IGenerator
{
    private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

    public string SubcommandName => "TranslateKeys";

    /// <summary>
    /// Generate translation for file
    /// </summary>
    private static string _GenerateFor(string source, TranslationDeclares items
        , GeneratorOption option,ref bool addGeneratedCodeAttribute)
    {
        CsBuilder builder = new(null,source);

        builder.Using.Add("Utopia.Core.Transition");
        builder.Namespace = option.Configuration.TranslationConfiguration.TargetNamespace;

        builder.EmitClass(option.Configuration.TranslationConfiguration.TargetClass,
            isPartial: true, isStatic: true, isPublic: true,
            addGeneratedCodeAttribute: addGeneratedCodeAttribute,parentClass: []);

        addGeneratedCodeAttribute = false;

        foreach (var item in items.Translations)
        {
            builder.EmitField("public", "TranslateKey",
                item.Id.Guuid.ToCsIdentifier(),
                defaultValue: $"TranslateKey.Create(\"{item.Id.Guuid}\",\"{item.Comment}\",\"{item.ProviderId.Guuid}\")",
                isReadonly: true, isStatic: true);
        }

        builder.CloseCodeBlock();

        return builder.Generate();
    }

    public void Execute(GeneratorOption option)
    {
        // save changes
        option.TranslateManager.Save();

        // read all translation files in translate-root directory
        string[] files = Directory.GetFiles(option.CurrentFileSystem.TranslationDirectory);

        // process
        XmlSerializer serializer = new(typeof(TranslationDeclares));
        var fs = option.CurrentFileSystem;

        bool addGeneratedCodeAttribute = true;

        foreach (string file in files)
        {
            try
            {
                using var xml = File.OpenRead(file);
                var declares = (TranslationDeclares?)serializer.Deserialize(xml)
                    ?? throw new XmlException("XmlSerializer.Deserialize return null");

                string output = fs.GetGeneratedCsFilePath(file);

                Directory.CreateDirectory(Path.GetDirectoryName(output)!);

                string generated = _GenerateFor(file, declares, option,ref addGeneratedCodeAttribute);

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
