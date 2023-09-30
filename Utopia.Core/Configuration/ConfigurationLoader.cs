using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tomlyn;
using Utopia.Core.Utilities.IO;

namespace Utopia.Core.Configuration;
public static class ConfigurationLoader
{

    private static string _GetFile<T>(IPluginInformation plugin,IFileSystem system)
    {
        // get path
        var path = system.GetConfigurationOfPlugin(plugin);

        Directory.CreateDirectory(path);

        // get attribute
        var type = typeof(T);

        var config = type.GetCustomAttribute<ConfigurationAttribute>();

        if (config == null)
        {
            throw new ArgumentException("the configuration type has no " + nameof(ConfigurationAttribute) + " attribute");
        }

        var file = Path.Join(path, config.FileName);

        return file;
    }

    /// <summary>
    /// Load config for a type,which has <see cref="ConfigurationAttribute"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="plugin"></param>
    /// <param name="system"></param>
    /// <returns></returns>
    public static T Load<T>(IPluginInformation plugin,IFileSystem system) where T : class, new() 
    {
        var toml = _GetFile<T>(plugin, system);
        return Toml.ToModel<T>(File.ReadAllText(toml,Encoding.UTF8));
    }

    public static void Store<T>(IPluginInformation plugin,IFileSystem system,T config)
    {
        ArgumentNullException.ThrowIfNull(config);
        var toml = _GetFile<T>(plugin,system);

        File.WriteAllText(toml, Toml.FromModel(config));
    }
}
