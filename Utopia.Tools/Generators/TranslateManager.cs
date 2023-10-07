namespace Utopia.Tools.Generators;

public enum TranslateItemType
{
    PluginInformation,
    Entity,
    Other
}

/// <summary>
/// This class will manage the translate files of the game/plugin.
/// </summary>
public sealed class TranslateManager
{

    private readonly IFileSystem _fileSystem;

    public TranslateManager(IFileSystem fileSystem)
    {
        this._fileSystem = fileSystem;
    }

    private static void _EnsureFile(string path)
    {
        if(!File.Exists(path))
        {
            File.Create(path);
        }
    }

    private void _EnsureFile(string origin,string path)
    {
        var p = Path.GetFullPath(Path.Join(this._fileSystem.TranslateDir,Path.GetRelativePath(origin,path)));

        _EnsureFile(p);
    }

    public void EnsurePluginInformationTrnaslate()
    {
        var path = this._fileSystem.GetTranslatedTomlFilePath("plugin.translated.toml");

        _EnsureFile(path);
    }

}
