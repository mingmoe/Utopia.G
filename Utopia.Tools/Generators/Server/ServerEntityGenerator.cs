// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Collections.Concurrent;
using System.Xml;
using System.Xml.Serialization;
using HarmonyLib;
using Microsoft.CodeAnalysis.Options;
using NLog;
using Tomlyn;
using Tomlyn.Model;
using Utopia.Core.Utilities;

namespace Utopia.Tools.Generators.Server;

public class ServerCustomEntity
{
    [XmlElement]
    public string Name { get; set; } = string.Empty;

    [XmlElement]
    public XmlElement Data { get; set; } = null!;
}

/// <summary>
/// The entity information of the generated.
/// </summary>
public class ServerEntityInfo
{
    [XmlElement]
    public bool Collidable { get; set; }

    [XmlElement]
    public bool Accessible { get; set; }

    [XmlElement]
    public XmlGuuid EntityId { get; set; } = new();

    /// <summary>
    /// This hint use which entity generator.
    /// </summary>
    [XmlChoiceIdentifier("Type")]
    [XmlElement("Common", typeof(string))]
    [XmlElement("Custom", typeof(ServerCustomEntity))]
    public object Data { get; set; } = null!;

    [XmlIgnore]
    public StandardEntityType Type { get; set; } = StandardEntityType.None;
}

/// <summary>
/// Those entity type was supported by office.
/// </summary>
public enum StandardEntityType
{
    None,
    /// <summary>
    /// stand for <see cref="CommonServerEntityGenerator"/>
    /// </summary>
    Common,
    /// <summary>
    /// stand for user custom entity type
    /// </summary>
    Custom,
}

/// <summary>
/// This generator will generate the entity class.
/// Just because it's a hard job. So we need many other entity generators to generate diverse entity.
/// We also need multiple threads.
/// </summary>
public class ServerEntityGenerator : IGenerator
{
    private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

    public int ThreadCount { get; set; }

    public ConcurrentDictionary<string, IServerEntityGenerator> CustomGenerators { get; } = new();

    private readonly CommonServerEntityGenerator _commonGenerator = new();

    public string SubcommandName => "server-entity";

    private static IEnumerable<string> _GetAllFile(string dir)
    {
        dir = Path.GetFullPath(dir);
        foreach (string f in Directory.GetFiles(dir))
        {
            yield return f;
        }
        foreach (string d in Directory.GetDirectories(dir))
        {
            foreach (string f in _GetAllFile(d))
            {
                yield return f;
            }
        }
    }

    public void Execute(GeneratorOption option)
    {
        // find all .xml
        List<string> xmlDocuments = [];

        foreach (string toml in _GetAllFile(option.TargetProject.EntitiesDirectory))
        {
            if (toml.EndsWith(".xml"))
            {
                xmlDocuments.Add(toml);
            }
        }

        // parse
        XmlSerializer xml = new(typeof(ServerEntityInfo));
        foreach (string xmlDocument in xmlDocuments)
        {
            try
            {
                using FileStream fs = new(xmlDocument,FileMode.Open);
                var obj = (ServerEntityInfo)(xml.Deserialize(fs) ?? throw new XmlException("XmlSerializer.Deserialize return null"));

                // find type
                switch(obj.Type)
                {
                    case StandardEntityType.None:
                        throw new InvalidDataException("the type of the entity xml file is None");
                    case StandardEntityType.Common:
                        _commonGenerator.Generate(xmlDocument, obj,option);
                        break;
                    case StandardEntityType.Custom:
                        var info = ((ServerCustomEntity)obj.Data);
                        if(!CustomGenerators.TryGetValue(info.Name, out IServerEntityGenerator? generator))
                        {
                            throw new InvalidDataException(
                                "the custom generator of the entity xml file can not be found." +
                                "ensure you have added the right custom generators");
                        }
                        break;
                }
            }
            catch (Exception)
            {
                s_logger.Error("get a exception when process file:{xml}", xmlDocument);
                throw;
            }
        }
    }
}
