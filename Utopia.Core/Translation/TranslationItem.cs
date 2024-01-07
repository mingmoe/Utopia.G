// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Utopia.Core.Translation;

public record TranslationDeclareItem(string Text, string Comment)
{
}

/// <summary>
/// This class was used for code generated and human edit.
/// </summary>
public sealed class TranslationItem
{
    /// <summary>
    /// The translation id of this item.
    /// </summary>
    [XmlElement]
    public string Text { get; set; } = "";

    /// <summary>
    /// The translation comment of this id.
    /// </summary>
    [XmlElement]
    public string Comment { get; set; } = string.Empty;

    /// <summary>
    /// The translated of this id.
    /// </summary>
    [XmlElement]
    public string Translated { get; set; } = string.Empty;
}

[XmlRoot(nameof(TranslationItems),Namespace = Xml.Namespace)]
public class TranslationItems
{
    public const string TranslationsElementName = "Translations";

    public const string TranslationItemElementName = "Translation";

    [XmlArray(TranslationsElementName)]
    [XmlArrayItem(TranslationItemElementName)]
    public List<TranslationItem> Translations { get; set; } = [];
}

public sealed class TranslationProject(IDictionary<string, string> items)
{
    public LanguageID Identification { get; set; } = new();

    public FrozenDictionary<string, string> Items { get; set; } = items.ToFrozenDictionary();
}
