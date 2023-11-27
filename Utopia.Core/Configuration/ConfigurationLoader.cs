// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Utopia.Core.Plugin;
using Utopia.Core.Utilities.IO;

namespace Utopia.Core.Configuration;

public class ConfigurationLoader
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
        string xml = _GetFile<T>(plugin, system);
        XmlSerializer serializer = new(typeof(T));

        using var fs = File.OpenRead(xml);

        return (T?)serializer.Deserialize(fs) ?? throw new XmlException("XmlSerializer.Deserialize return null");
    }

    public static void Store<T>(IPluginInformation plugin, IFileSystem system, T config)
    {
        ArgumentNullException.ThrowIfNull(config);
        string xml = _GetFile<T>(plugin, system);

        XmlSerializer serializer = new(typeof(T));

        using var fs = File.OpenWrite(xml);

        serializer.Serialize(fs,config);
    }
}
