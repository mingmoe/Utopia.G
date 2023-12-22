// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Collections.Frozen;
using System.Xml.Serialization;
using Utopia.Core.Utilities;

namespace Utopia.Core.Translation;

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

public class TranslationDeclares
{
    [XmlArray("Translations")]
    [XmlArrayItem("Translation")]
    public List<TranslationItem> Translations { get; set; } = [];
}

public sealed class TranslationProject(IDictionary<string, string> items)
{
    public TranslateIdentifence Identification { get; set; } = new();

    public FrozenDictionary<string, string> Items { get; set; } = items.ToFrozenDictionary();
}

internal class TranslationProvider : ITranslateProvider
{
    private TranslationProject _Project { get; init; }

    public TranslationProvider(TranslationProject project)
    {
        ArgumentNullException.ThrowIfNull(project);
        _Project = project;
    }

    public bool Contain(TranslateIdentifence language, string item) => _Project.Identification.Equals(language) && _Project.Items.ContainsKey(item);

    public bool TryGetItem(TranslateIdentifence language, string item, out string? result)
    {
        if (!_Project.Identification.Equals(language))
        {
            result = null;
            return false;
        }

        if (_Project.Items.TryGetValue(item, out result))
        {
            result = null;
            return true;
        }
        else
        {
            return false;
        }
    }
}
