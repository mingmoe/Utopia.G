// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.CodeAnalysis.CSharp;
using NLog;
using Utopia.Core.Translation;
using Utopia.Core.Utilities;

namespace Utopia.Tool.Generators;

public class TranslationConfiguration
{
    [XmlElement]
    public string TargetNamespace { get; set; } = "global";

    [XmlElement]
    public string TargetClass { get; set; } = "TranslationKeys";

    [XmlElement]
    public string OutputFileClassification { get; set; } = "TranslationKeys";
}

public class TranslateKeyGenerator : IGenerator
{
    private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

    public string SubcommandName => "TranslateKeys";

    public void Execute(GeneratorOption option)
    {
        string getterName = "@_Getter_";
        // generate source file
        CsBuilder builder = new();
        builder.Using.Add("Utopia.Core.Translation");
        builder.Namespace = option.Configuration.TranslationConfiguration.TargetNamespace;
        builder.EmitClass(
            option.Configuration.TranslationConfiguration.TargetClass,
            isPublic: true,
            isStatic: false,
            isPartial: false);
        builder.EmitProperty(
            "public",
            "ITranslationGetter",
            getterName,
            accessor: "{ private get; init; }",
            isRequired: true);
        {
            var items = option.TranslateManager.Translations.ToArray();

            foreach(var item in items)
            {
                builder.EmitProperty(
                    "public",
                    "string",
                    item.Key,
                    $" => {getterName}.I18n({SymbolDisplay.FormatLiteral(item.Value.text,true)},{SymbolDisplay.FormatLiteral(item.Value.comment, true)});");
            }
        }
        builder.CloseCodeBlock();

        var got = builder.Generate();

        var file = option.CurrentFileSystem.GetGeneratedCsFilePath(option.Configuration.TranslationConfiguration.OutputFileClassification);

        File.WriteAllText(file, got, Encoding.UTF8);
    }

    public static string AccessTranslation(GeneratorOption option,string cSharpId)
    {
        return $"{option.Configuration.TranslationConfiguration.TargetNamespace}" +
            $".{option.Configuration.TranslationConfiguration.TargetClass}" +
            $".{cSharpId}";
    }

    public static string InjectTranslationGetter(CsBuilder builder)
    {
        builder.EmitProperty(
            "public",
            "ITranslationGetter",
            "_TranslationGetter_",
            accessor: "{ private get; init; }",
            isRequired: true);
        return "_TranslationGetter_";
    }
}
