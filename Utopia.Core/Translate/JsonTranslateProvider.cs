using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Utopia.Core.Utilities;

namespace Utopia.Core.Translate;

public class JsonTranslateProject
{
    public TranslateIdentifence Identifence { get; set; } = new();

    public ImmutableDictionary<Guuid, string> Items { get; set; } = ImmutableDictionary<Guuid, string>.Empty;
}

public class JsonTranslateProvider : ITranslateProvider
{
    private JsonTranslateProject _Project { get; init; }

    public JsonTranslateProvider(JsonTranslateProject project)
    {
        ArgumentNullException.ThrowIfNull(project);
        this._Project = project;
    }

    public static JsonTranslateProvider LoadFile(string path)
    {
        ArgumentNullException.ThrowIfNull(path);
        var read = File.ReadAllText(path,Encoding.UTF8);

        return new(
            JsonSerializer.Deserialize<JsonTranslateProject>(read) ??
            throw new IOException("failed to parse file"));
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

        if(this._Project.Items.TryGetValue(id, out result))
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
