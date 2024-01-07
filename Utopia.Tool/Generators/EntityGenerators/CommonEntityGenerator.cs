// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Utopia.Tool.Generators.EntityGenerators;

public class CommonEntityGenerator : IEntityGenerator
{
    public void Generate(string sourcePath, EntityInformation info, XmlElement data, GeneratorOption option)
    {
        // no output any thing,but add transition
        option.TranslateManager.AddEntityTranslation(info);
        return;
    }
}
