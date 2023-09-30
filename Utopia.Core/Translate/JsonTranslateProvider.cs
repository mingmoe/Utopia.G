#region copyright
// This file(may named JsonTranslateProvider.cs) is a part of the project: Utopia.Core.
// 
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// 
// This file is part of Utopia.Core.
//
// Utopia.Core is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Utopia.Core is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with Utopia.Core. If not, see <https://www.gnu.org/licenses/>.
#endregion

using System.Collections.Immutable;
using System.Text;
using System.Text.Json;
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
        var read = File.ReadAllText(path, Encoding.UTF8);

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
