// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utopia.Tools.Generators;

public class TranslationProviderConfiguration
{
    public string LoaderClassName = "TranslationProvider";

    public string LoadFromDirectory = IPluginDevFileSystem.DefaultTranslationDirectoryName;
}

public class TranslationLoaderGenerator : IGenerator
{
    public string SubcommandName => "server-plugin-translation-loader";

    public void Execute(GeneratorOption option)
    {
        
    }
}
