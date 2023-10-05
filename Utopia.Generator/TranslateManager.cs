using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utopia.Core.Translate;

namespace Utopia.Generator;

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
    public static string GetTranslateDirectory(TranslateIdentifence? identifence = null)
    {
        return Path.GetFullPath(Path.Join(TranslateDirectory,identifence?.ToString() ?? "root"));
    }

    public static string GetTranslateFileOf(string fileName,TranslateIdentifence? identifence)
    {
        return Path.Join(
            GetTranslateDirectory(identifence),
            fileName);
    }
}
