// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Utopia.Tools.Generators.Server;
public class CommonServerEntityGenerator : IServerEntityGenerator
{
    public void Generate(string tomlPath, ServerEntityInfo info,XmlElement data, GeneratorOption option)
    {
        // no output any thing,but add transition
        var adder = option.TranslateManager.GetTransitionAdder(TranslateItemType.Entity);

        adder.Invoke(GuuidManager.GetEntityNameTransitionOf(
            option.Configuration.PluginInformation.PluginId.Guuid,
            info.EntityId.Guuid), "the name of the entity");
        adder.Invoke(GuuidManager.GetEntityDescriptionTransitionOf(
            option.Configuration.PluginInformation.PluginId.Guuid,
            info.EntityId.Guuid),"the description of the entity");

        return;
    }
}
