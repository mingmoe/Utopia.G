// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Text;
using Tomlyn;
using Utopia.Core.Utilities;

namespace Utopia.Tools.Generators;

public enum TranslateItemType
{
    PluginInformation,
    Entity,
    Other
}

/// <summary>
/// Only declare a existence of a transition item,no any translated content.
/// </summary>
public class TranslationDeclare
{
    public Guuid Provider { get; set; } = Guuid.Empty;

    public string Comment { get; set; } = string.Empty;
}

/// <summary>
/// This class will manage the translate files of the game/plugin.
/// Must call <see cref="Dispose"/> to save the translation files.
/// </summary>
public sealed class TranslateManager : IDisposable
{

    private readonly IPluginDevFileSystem _fileSystem;

    private readonly Dictionary<TranslateItemType, Dictionary<Guuid, TranslationDeclare>?> _translates = new();

    private readonly TranslateItemType[] _keys;

    /// <summary>
    /// Ensure that all values of <see cref="_translates"/> is not null.
    /// </summary>
    private void _Read()
    {
        Dictionary<Guuid, TranslationDeclare> read(TranslateItemType type)
        {
            string path = _fileSystem.GetTranslatedTomlFilePath(type);
            TomlModelOptions option = Guuid.AddTomlOption();

            _EnsureFile(path);

            return Toml.ToModel<Dictionary<Guuid, TranslationDeclare>>(
                File.ReadAllText(path), options: option);
        }

        foreach (TranslateItemType key in _keys)
        {
            if (_translates[key] == null)
            {
                _translates[key] = read(key);
            }
        }
    }

    private void _Write()
    {
        void write(TranslateItemType type, Dictionary<Guuid, TranslationDeclare> model)
        {
            string path = _fileSystem.GetTranslatedTomlFilePath(type);
            TomlModelOptions option = Guuid.AddTomlOption();

            if (File.Exists(path))
            {
                File.Delete(path);
            }
            _EnsureFile(path);

            File.WriteAllText(path, Toml.FromModel(
                model, options: option), Encoding.UTF8);
        }

        foreach (TranslateItemType key in _keys)
        {
            if (_translates[key] != null)
            {
                write(key, _translates[key]!);
            }
        }
    }

    /// <summary>
    /// Read translation files
    /// </summary>
    public void Load() => _Read();

    /// <summary>
    /// Write translation files. This was called automatically when call <see cref="Dispose"/>
    /// </summary>
    public void Save() => _Write();

    public TranslateManager(IPluginDevFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
        _keys = Enum.GetValues<TranslateItemType>();

        foreach (TranslateItemType item in _keys)
        {
            _translates.Add(item, null!);
        }
    }

    private static void _EnsureFile(string path)
    {
        if (!File.Exists(path))
        {
            File.Create(path).Close();
        }
    }

    /// <summary>
    /// Ensure the translate item exists in the toml table.
    /// Otherwise create it.
    /// </summary>
    private static void _EnsureTranslateKey(IDictionary<Guuid, TranslationDeclare> model, Guuid id, Guuid provider, string comment)
    {
        if (model.ContainsKey(id))
        {
            return;
        }

        model.Add(id, new TranslationDeclare
        {
            Provider = provider,
            Comment = comment
        });
    }

    public Action<Guuid, string> GetEnsurer(TranslateItemType type)
    {
        _Read();

        return (Guuid id, string comment) =>
        {
            EnsureTranslate(type, id, comment);
        };
    }

    public void EnsurePluginInformationTrnaslate()
    {
        Action<Guuid, string> ensurer = GetEnsurer(TranslateItemType.PluginInformation);

        Guuid id = _fileSystem.ReadPluginInfo().Id;

        ensurer.Invoke(GuuidManager.GetPluginNameTranslateId(id), "the translation of the name");
        ensurer.Invoke(GuuidManager.GetPluginDescriptionTranslateId(id), "the translation of the description");
    }

    public void EnsureTranslate(TranslateItemType type, Guuid transletKeyid, string comment)
    {
        ArgumentNullException.ThrowIfNull(transletKeyid);
        ArgumentNullException.ThrowIfNull(comment);

        _Read();

        PluginInfo pluginInfo = _fileSystem.ReadPluginInfo();
        Guuid id = pluginInfo.Id;
        Guuid provider = GuuidManager.GetTranslateProviderGuuidOf(id);

        _EnsureTranslateKey(_translates[type]!, transletKeyid, provider, comment);
    }

    public void Dispose()
    {
        Save();
        GC.SuppressFinalize(this);
    }
}
