// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Autofac.Core;
using Utopia.Core.Translation;
using Utopia.Core.Utilities;

namespace Utopia.Tools.Generators;

public enum TranslateItemType
{
    PluginInformation,
    Entity,
    Other
}

/// <summary>
/// This class will manage the translations files of the game/plugin.
/// Must call <see cref="Dispose"/> to save the translation files.
/// </summary>
public sealed class TranslateManager : IDisposable
{
    private bool _disposed = false;

    private readonly XmlSerializer _serializer = new(typeof(TranslationDeclares));

    private readonly IPluginDevFileSystem _fileSystem;

    private readonly Configuration _configuration;

    private readonly Dictionary<TranslateItemType, Dictionary<string, TranslationItem>?> _translates = new();

    private readonly TranslateItemType[] _keys;

    /// <summary>
    /// Ensure that all values of <see cref="_translates"/> is not null.
    /// </summary>
    private void _Read()
    {
        Dictionary<string, TranslationItem> read(TranslateItemType type)
        {
            string path = _fileSystem.GetTranslatedXmlFilePath(type);

            if (_EnsureFile(path))
            {
                return new();
            }

            using var fs = new FileStream(path, FileMode.Open);

            if(fs.Length == 0)
            {
                return new();
            }

            var obj = (TranslationDeclares)(_serializer.Deserialize(fs) ?? throw new XmlException("XmlSerializer.Deserialize returns null"));

            return obj.Translations.ToDictionary((item) => { return item.Text; });
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
        void write(TranslateItemType type, Dictionary<string, TranslationItem> model)
        {
            string path = _fileSystem.GetTranslatedXmlFilePath(type);

            if (File.Exists(path))
            {
                File.Delete(path);
            }
            _EnsureFile(path);

            using var fs = new FileStream(path, FileMode.Open);

            var declares = new TranslationDeclares();
            foreach(KeyValuePair<string, TranslationItem> item in model)
            {
                declares.Translations.Add(item.Value);
            }

            _serializer.Serialize(fs, declares);
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

    public TranslateManager(Configuration configuration,IPluginDevFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
        _configuration = configuration;
        _keys = Enum.GetValues<TranslateItemType>();

        foreach (TranslateItemType item in _keys)
        {
            _translates.Add(item, null!);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <returns>If return true, the file has existed</returns>
    private static bool _EnsureFile(string path)
    {
        if (!File.Exists(path))
        {
            File.Create(path).Close();
            return false;
        }
        return true;
    }

    /// <summary>
    /// Ensure the translate item exists in the toml table.
    /// Otherwise create it.
    /// </summary>
    private static void _EnsureTranslateKey(IDictionary<string, TranslationItem> model, string item, string comment)
    {
        if (model.ContainsKey(item))
        {
            return;
        }

        model.Add(item, new TranslationItem
        {
            Text = item,
            Comment = comment
        });
    }

    public Action<string, string> GetTransitionAdder(TranslateItemType type)
    {
        _Read();

        return (string id, string comment) =>
        {
            EnsureTranslate(type, id, comment);
        };
    }

    public void EnsurePluginInformationTransition()
    {
        Action<string, string> adder = GetTransitionAdder(TranslateItemType.PluginInformation);

        adder.Invoke(_configuration.PluginInformation.Name, "the translation of the name");
        adder.Invoke(_configuration.PluginInformation.Description, "the translation of the description");
        Save();
    }

    public void EnsureTranslate(TranslateItemType type, string item, string comment)
    {
        ArgumentNullException.ThrowIfNull(item);
        ArgumentNullException.ThrowIfNull(comment);

        _Read();

        Guuid id = _configuration.PluginInformation.Id.Guuid;

        _EnsureTranslateKey(_translates[type]!, item, comment);
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

        if (disposing)
        {
            Save();
        }

        _disposed = true;
    }
}
