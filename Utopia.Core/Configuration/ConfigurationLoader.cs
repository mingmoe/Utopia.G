// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Reflection;
using System.Text;
using Tomlyn;
using Utopia.Core.Plugin;
using Utopia.Core.Utilities.IO;

namespace Utopia.Core.Configuration;
public static class ConfigurationLoader
{

    private static string _GetFile<T>(IPluginInformation plugin, IFileSystem system)
    {
        // get path
        string path = system.GetConfigurationDirectoryOfPlugin(plugin);

        _ = Directory.CreateDirectory(path);

        // get attribute
        Type type = typeof(T);

        ConfigurationAttribute config = type.GetCustomAttribute<ConfigurationAttribute>() ??
            throw new ArgumentException("the configuration type has no " + nameof(ConfigurationAttribute) + " attribute");

        string file = Path.Join(path, config.FileName);

        return file;
    }

    /// <summary>
    /// Load config for a type,which has <see cref="ConfigurationAttribute"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="plugin"></param>
    /// <param name="system"></param>
    /// <returns></returns>
    public static T Load<T>(IPluginInformation plugin, IFileSystem system) where T : class, new()
    {
        string toml = _GetFile<T>(plugin, system);
        return Toml.ToModel<T>(File.ReadAllText(toml, Encoding.UTF8));
    }

    public static void Store<T>(IPluginInformation plugin, IFileSystem system, T config)
    {
        ArgumentNullException.ThrowIfNull(config);
        string toml = _GetFile<T>(plugin, system);

        File.WriteAllText(toml, Toml.FromModel(config));
    }
}
