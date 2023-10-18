using Esprima.Ast;
using System.Text;
using Tomlyn;
using Tomlyn.Model;
using Utopia.Core.Translate;
using Utopia.Core.Utilities;

namespace Utopia.Tools.Generators;

public enum TranslateItemType
{
    PluginInformation,
    Entity,
    Other
}

/// <summary>
/// This class will manage the translate files of the game/plugin.
/// Must call <see cref="Dispose"/> to save the translation files.
/// </summary>
public sealed class TranslateManager : IDisposable
{

    private readonly IFileSystem _fileSystem;

    private readonly Dictionary<TranslateItemType, Dictionary<Guuid, TomlTranslateHumanItem>?> _translates = new();

    private readonly TranslateItemType[] _keys;

    /// <summary>
    /// Ensure that all values of <see cref="_translates"/> is not null.
    /// </summary>
    private void _Read()
    {
        Dictionary<Guuid, TomlTranslateHumanItem> read(TranslateItemType type)
        {
            var path = this._fileSystem.GetTranslatedTomlFilePath(type);
            var option = Guuid.AddTomlOption();

            _EnsureFile(path);

            return Toml.ToModel<Dictionary<Guuid, TomlTranslateHumanItem>>(
                File.ReadAllText(path), options: option);
        }

        foreach(var key in this._keys)
        {
            if (this._translates[key] == null)
            {
                this._translates[key] = read(key);
            }
        }
    }

    private void _Write()
    {
        void write(TranslateItemType type, Dictionary<Guuid, TomlTranslateHumanItem> model)
        {
            var path = this._fileSystem.GetTranslatedTomlFilePath(type);
            var option = Guuid.AddTomlOption();

            if (File.Exists(path))
            {
                File.Delete(path);
            }
            _EnsureFile(path);

            File.WriteAllText(path, Toml.FromModel(
                model, options: option),Encoding.UTF8);
        }

        foreach (var key in this._keys)
        {
            if (this._translates[key] != null)
            {
                write(key, this._translates[key]!);
            }
        }
    }

    /// <summary>
    /// Read translation files
    /// </summary>
    public void Load()
    {
        this._Read();
    }

    /// <summary>
    /// Write translation files. This was called automatically when call <see cref="Dispose"/>
    /// </summary>
    public void Save()
    {
        this._Write();
    }

    public TranslateManager(IFileSystem fileSystem)
    {
        this._fileSystem = fileSystem;
        this._keys = Enum.GetValues<TranslateItemType>();

        foreach (var item in this._keys)
        {
            this._translates.Add(item, null!);
        }
    }

    private static void _EnsureFile(string path)
    {
        if(!File.Exists(path))
        {
            File.Create(path).Close();
        }
    }

    /// <summary>
    /// Ensure the translate item exists in the toml table.
    /// Otherwise create it.
    /// </summary>
    private static void _EnsureTranslateKey(IDictionary<Guuid, TomlTranslateHumanItem> model,Guuid id,Guuid provider,string comment)
    {
        if(model.ContainsKey(id))
        {
            return;
        }

        model.Add(id, new TomlTranslateHumanItem
        {
            Provider = provider,
            Comment = comment,
            Translated = string.Empty
        });
    }

    public Action<Guuid,string> GetEnsurer(TranslateItemType type)
    {
        this._Read();

        return (Guuid id, string comment) =>
        {
            this.EnsureTranslate(type, id, comment);
        };
    }

    public void EnsurePluginInformationTrnaslate()
    {
        var ensurer = this.GetEnsurer(TranslateItemType.PluginInformation);

        var id = this._fileSystem.ReadPluginInfo().Id;

        ensurer.Invoke(GuuidManager.GetPluginNameTranslateId(id), "the translation of the name");
        ensurer.Invoke(GuuidManager.GetPluginDescriptionTranslateId(id), "the translation of the description");
    }

    public void EnsureTranslate(TranslateItemType type,Guuid transletKeyid,string comment)
    {
        ArgumentNullException.ThrowIfNull(transletKeyid);
        ArgumentNullException.ThrowIfNull(comment);

        this._Read();

        var pluginInfo = this._fileSystem.ReadPluginInfo();
        var id = pluginInfo.Id;
        var provider = GuuidManager.GetTranslateProviderGuuidOf(id);

        _EnsureTranslateKey(this._translates[type]!, transletKeyid, provider, comment);
     }

    public void Dispose()
    {
        this.Save();
        GC.SuppressFinalize(this);
    }
}
