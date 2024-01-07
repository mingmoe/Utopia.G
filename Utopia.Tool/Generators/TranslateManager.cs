// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Autofac.Core;
using Utopia.Core.Collections;
using Utopia.Core.Translation;
using Utopia.Core.Utilities;

namespace Utopia.Tool.Generators;

/// <summary>
/// This class will manage the translations files of the game/plugin.
/// Must call <see cref="Dispose"/> to save the translation files.
/// </summary>
public sealed class TranslateManager : IDisposable
{
    private bool _disposed = false;

    public readonly SafeDictionary<string, (string text,string comment)> Translations = new();

    public string GetCSIdOfEntityName(Guuid entity)
    {
        return entity.ToCsIdentifier() + "_NAME";
    }

    public string GetCSIdOfEntityDescription(Guuid entity)
    {
        return entity.ToCsIdentifier() + "_DESCRIPTION";
    }

    public TranslationDeclareItem GetNameDeclareOfEntity(Guuid entityId,string text)
    {
        return new(text, $"The name of the entity:{entityId}.");
    }
    public TranslationDeclareItem GetDescriptionDeclareOfEntity(Guuid entityId,string text)
    {
        return new(text, $"The description of the entity:{entityId}.");
    }

    public void AddItem(string text, string comment,string csharpId)
    {
        Translations.TryAdd(csharpId, new(text, comment));
    }

    public void AddItem(TranslationDeclareItem item,string csharpId)
    {
        Translations.TryAdd(csharpId, new(item.Text, item.Comment));
    }

    /// <summary>
    /// Only add name and description translation for the entity.
    /// </summary>
    public void AddEntityTranslation(EntityInformation entity)
    {
        AddItem(
            GetNameDeclareOfEntity(entity.EntityId.Guuid, entity.EntityName),
            GetCSIdOfEntityName(entity.EntityId.Guuid));
        AddItem(
            GetDescriptionDeclareOfEntity(entity.EntityId.Guuid, entity.EntityDescription),
            GetCSIdOfEntityDescription(entity.EntityId.Guuid));
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
    }
}
