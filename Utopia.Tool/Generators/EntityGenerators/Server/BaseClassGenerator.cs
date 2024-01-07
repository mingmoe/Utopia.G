// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Utopia.Core.Utilities;

namespace Utopia.Tool.Generators.EntityGenerators.Server;

/// <summary>
/// Generate a `Entity` class
/// </summary>
public class BaseClassGenerator : IEntityGenerator
{
    public static string GetEntityClassName(Guuid entityId) => $"Entity_{entityId.ToCsIdentifier()}";

    public void Generate(string sourcePath, EntityInformation info, XmlElement data, GeneratorOption option)
    {
        CsBuilder builder = new();
        builder.Using.Add("Utopia.Server.Entity");

        builder.EmitClass(
            className: GetEntityClassName(info.EntityId.Guuid),
            isPublic: false,
            isPartial:true,
            parentClass: "IEntity");

        TranslateKeyGenerator.InjectTranslationGetter(builder);

        var output = option.CurrentFileSystem.GetGeneratedCsFilePath(
            option.CurrentFileSystem.EntitiesDirectory, sourcePath, "base");

        File.WriteAllText(output, builder.Generate(), Encoding.UTF8);
    }
}
