using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Utopia.Core.Utilities;

namespace Utopia.Core.Translate;

public sealed class TomlTranslateProject
{
    public TranslateIdentifence Identifence { get; set; } = new();

    public Dictionary<Guuid, string> Items { get; set; } = new();
}

class TomlTranslateProvider : ITranslateProvider
{
    private TomlTranslateProject _Project { get; init; }

    public TomlTranslateProvider(TomlTranslateProject project)
    {
        ArgumentNullException.ThrowIfNull(project);
        this._Project = project;
    }

    public bool Contain(TranslateIdentifence language, Guuid id)
    {
        if (!this._Project.Identifence.Equals(language))
        {
            return false;
        }

        return this._Project.Items.ContainsKey(id);
    }

    public bool TryGetItem(TranslateIdentifence language, Guuid id, out string? result)
    {
        if (!this._Project.Identifence.Equals(language))
        {
            result = null;
            return false;
        }

        if (this._Project.Items.TryGetValue(id, out result))
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
