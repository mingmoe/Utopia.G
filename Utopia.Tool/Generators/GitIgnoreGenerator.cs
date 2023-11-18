// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utopia.Tools.Generators;

namespace Utopia.Tool.Generators;
public class GitIgnoreGenerator : IGenerator
{
    public string SubcommandName => "GitIgnore";

    public void Execute(GeneratorOption option)
    {
        var dir = option.CurrentFileSystem.ProjectRootDir;

        var file = Path.GetFullPath(".gitignore", dir);

        Dictionary<string, bool> items = new();
        foreach (var item in (string[])[
            Path.GetFullPath(option.CurrentFileSystem.GeneratedDirectory) + '/', // directory
            Path.GetFullPath(option.CurrentFileSystem.GetTranslatedXmlFilePath(TranslateItemType.PluginInformation)),
            Path.GetFullPath(option.CurrentFileSystem.GetTranslatedXmlFilePath(TranslateItemType.Entity)),
            Path.GetFullPath(option.CurrentFileSystem.GetTranslatedXmlFilePath(TranslateItemType.Other))
        ])
        {
            items.Add(item.Replace("\\","/"), false);
        }

        foreach(var line in File.ReadLines(file))
        {
            if (line.StartsWith("#"))
            {
                continue;
            }
            var got = Path.GetFullPath(line, dir);
            got = got.Replace("\\", "/");

            if(items.TryGetValue(got,out bool c))
            {
                items.Remove(got);
                items.Add(got, true);
            }
        }

        // add no exists items
        var notFound = items.TakeWhile((k) => !k.Value).ToList();

        foreach(var item in notFound)
        {
            File.AppendAllText(file, $"\n{Path.GetRelativePath(dir,item.Key).Replace('\\','/')}\n",Encoding.UTF8);
        }
    }
}
