// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Utopia.Core.Utilities;

namespace Utopia.Core.Translation;

public static class TranslationLoader
{
    /// <summary>
    /// Load a .xml translation file,its type as <see cref="TranslationDeclares"/>
    /// </summary>
    public static Dictionary<string, string> LoadFromFile(string file)
    {
        XmlSerializer serializer = new(typeof(TranslationDeclares));

        TranslationDeclares? declares = null;

        using (var fs = File.OpenText(file))
        {
            declares = (TranslationDeclares?)serializer.Deserialize(fs)
                ?? throw new XmlException("XmlSerializer.Deserialize return null");
        }

        Dictionary<string, string> items = [];

        foreach (var item in declares.Translations)
        {
            items.Add(item.Text, item.Translated);
        }

        return items;
    }

    /// <summary>
    /// Load all .xml files and union them into one.
    /// It use <see cref="LoadFromFile(string)"/> to read from file.
    /// </summary>
    public static Dictionary<string, string> LoadFromDirectory(string directory)
    {
        Dictionary<string, string> items = [];
        foreach (var files in Directory.GetFiles(Path.GetFullPath(directory), "*", SearchOption.AllDirectories))
        {
            if (files.EndsWith(".xml"))
            {
                items.Union(LoadFromFile(files));
            }
        }

        return items;
    }
}
