namespace Utopia.Tools.Generators;

/// <summary>
/// This class will manage the translate files of the game/plugin.
/// </summary>
public static class TranslateManager
{

    public const string TranslateDirectory = "translate";

    /// <summary>
    /// Get the translate directory.
    /// If you pass a null language identifence(by default),you will get a
    /// root-translate directory. The files under the directory wont
    /// be translated to any langugae. Those files just record what to be
    /// translated.Source generators should operate the root-translate directory.
    /// The target language will be chosen by the user.
    /// </summary>
    /// <param name="identifence"></param>
    /// <returns></returns>
    public static string GetTranslateDirectory(string? identifence = null)
    {
        return Path.GetFullPath(Path.Combine(TranslateDirectory, identifence?.ToString() ?? "root"));
    }

    public static string GetTranslateFileOf(string fileName, string? identifence)
    {
        return Path.Combine(
            GetTranslateDirectory(identifence),
            fileName);
    }
}
