// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Utopia.Tools.Generators;

public class TranslationProviderConfiguration
{
    [XmlElement]
    public string LoaderNamespace = "global";

    [XmlElement]
    public string LoaderClassName = "TranslationProvider";

    [XmlElement]
    public string LoadFromDirectory = IPluginDevFileSystem.DefaultTranslationDirectoryName;
}

public class TranslationLoaderGenerator : IGenerator
{
    public string SubcommandName => "TranslationLoader";

    public void Execute(GeneratorOption option)
    {
        CsBuilder builder = new();

        builder.Namespace = option.Configuration.TranslationProviderConfiguration.LoaderNamespace;

        builder.Using.Add("Utopia.Core");

        builder.EmitClass(option.Configuration.TranslationProviderConfiguration.LoaderClassName,
            isPublic: true,
            isStatic: false,
            isPartial: false,
            addGeneratedCodeAttribute: true,
            parentClass: []);

        // read translations
        builder.EmitLine("public static ");

        builder.CloseCodeBlock();

        var output = option.CurrentFileSystem.GetGeneratedCsFilePath(SubcommandName);

        File.WriteAllText(output, builder.Generate(), Encoding.UTF8);
    }
}
