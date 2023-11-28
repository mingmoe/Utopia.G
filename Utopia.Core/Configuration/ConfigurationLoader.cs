// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Utopia.Core.Collections;
using Utopia.Core.IO;
using Utopia.Core.Plugin;

namespace Utopia.Core.Configuration;

public class ConfigurationLoader : IConfigurationLoader
{
    private SafeDictionary<Type, XmlSerializer> _serializers = new();

    private SafeDictionary<Type, XmlSchemas> _schemas = new();

    private SafeDictionary<string, byte> _lastWrite = new();

    public required IPluginFileSystem PluginFileSystem { protected get; init; }

    private void GenerateXsd(Type type, PluginConfigurationAttribute attribute)
    {
        if (!attribute.GenerateXsdFile)
        {
            return;
        }

        var xsd = PluginFileSystem.GetConfigurationFilePathOfPlugin(attribute.XsdFilePath);

        var schemas =  _schemas.GetOrAdd(type, Xml.GetXmlSchema);

        var isNotCreated = _lastWrite.TryAdd(xsd, 0);

        if (!isNotCreated)
        {
            return;
        }

        using var fs = File.OpenWrite(xsd);
        Xml.WriteXmlSchemas(schemas, fs);
    }

    public T Load<T>(string? path)
    {
        var serializer = _serializers.GetOrAdd(typeof(T), (t) =>
        {
            return new XmlSerializer(t);
        });

        if (path == null)
        {
            var attribute = typeof(T).GetCustomAttribute<PluginConfigurationAttribute>()
                ?? throw new ArgumentException("the type has no ConfigurationAttribute and pass a null path");

            GenerateXsd(typeof(T), attribute);

            path = PluginFileSystem.GetConfigurationFilePathOfPlugin(attribute.FilePath);
        }

        using var fs = File.OpenText(path);

        return (T?)serializer.Deserialize(fs)
            ?? throw new XmlException("XmlSerializer.Deserialize() returns null");
    }

    public void Store<T>(T configuration,string? path)
    {
        var serializer = _serializers.GetOrAdd(typeof(T), (t) =>
        {
            return new XmlSerializer(t);
        });

        if(path == null)
        {
            var attribute = typeof(T).GetCustomAttribute<PluginConfigurationAttribute>()
                ?? throw new ArgumentException("the type has no ConfigurationAttribute and pass a null path");

            GenerateXsd(typeof(T), attribute);

            path = PluginFileSystem.GetConfigurationFilePathOfPlugin(attribute.FilePath);
        }

        using var fs = File.OpenWrite(path);
        serializer.Serialize(fs, configuration);
    }
}
