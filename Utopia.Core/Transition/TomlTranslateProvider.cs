// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using Utopia.Core.Utilities;

namespace Utopia.Core.Transition;

/// <summary>
/// This class was used for code generated and human edit.
/// </summary>
public sealed class TomlTranslateHumanItem
{
    public Guuid Provider { get; set; } = Guuid.Empty;

    public string Comment { get; set; } = string.Empty;

    public string Translated { get; set; } = string.Empty;
}

public sealed class TomlTranslateProject
{
    public TranslateIdentifence Identifence { get; set; } = new();

    public Dictionary<Guuid, string> Items { get; set; } = new();
}

internal class TomlTranslateProvider : ITranslateProvider
{
    private TomlTranslateProject _Project { get; init; }

    public TomlTranslateProvider(TomlTranslateProject project)
    {
        ArgumentNullException.ThrowIfNull(project);
        _Project = project;
    }

    public bool Contain(TranslateIdentifence language, Guuid id) => _Project.Identifence.Equals(language) && _Project.Items.ContainsKey(id);

    public bool TryGetItem(TranslateIdentifence language, Guuid id, out string? result)
    {
        if (!_Project.Identifence.Equals(language))
        {
            result = null;
            return false;
        }

        if (_Project.Items.TryGetValue(id, out result))
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
