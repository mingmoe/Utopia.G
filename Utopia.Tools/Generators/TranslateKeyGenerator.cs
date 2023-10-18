using NLog;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tomlyn;
using Utopia.Core.Translate;
using Utopia.Core.Utilities;

namespace Utopia.Tools.Generators;
public class TranslateKeyGenerator : IGenerator
{

    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public const string TranslationClassName = "TranslationKeys";

    public string SubcommandName => "translate-key";

    /// <summary>
    /// Generate translation for file
    /// </summary>
    private static string _GenerateFor(string source,Dictionary<Guuid,TomlTranslateHumanItem> items
        ,GeneratorOption option)
    {
        CsBuilder builder = new(source);

        builder.Usings.Add("Utopia.Core.Translate");
        builder.Namespace = option.TargetNamespace;

        builder.AddClass(TranslationClassName, isPartial: true, isStatic: true, isPublic: true);

        foreach (var item in items)
        {
            builder.AddField("public", "TranslateKey", item.Key.ToCsIdentifier(),
                defaultValue: $"TranslateKey.Create(\"{item.Key}\",\"{item.Value.Comment}\",\"{item.Value.Provider}\")", isReadonly: true,isStatic: true);
        }

        builder.CloseCodeBlock();
 
        return builder.Generate();
   }

    public void Execute(GeneratorOption option)
    {
        // read all translation files in translate-root directory
        var files = Directory.GetFiles(option.TargetProject.TranslateDir);

        // process
        var tomlOpt = Guuid.AddTomlOption();
        var fs = option.TargetProject;

        foreach(var file in files)
        {
            try
            {
                var item = Toml.ToModel<Dictionary<Guuid, TomlTranslateHumanItem>>(
                    File.ReadAllText(file, Encoding.UTF8), options: tomlOpt);

                var output = fs.GetGeneratedCsFilePath(file);

                Directory.CreateDirectory(Path.GetDirectoryName(output)!);

                var generated = _GenerateFor(file, item, option);

                File.WriteAllText(output, generated,Encoding.UTF8);
            }
            catch(Exception ex)
            {
                _logger.Error(ex, "get an error when process file {file}",file);
                throw;
            }
        }
    }
}
