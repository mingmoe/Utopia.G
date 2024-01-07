// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using NLog;
using System.Reflection;
using System.Xml.Linq;

namespace Utopia.Tool.Generators;

/// <summary>
/// This generator will generate the entity class.
/// Just because it's a hard job. So we need many other entity generators to generate diverse entity.
/// We also need multiple threads.
/// </summary>
public class EntityGenerator : IGenerator
{
    private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

    public int ThreadCount { get; set; }

    private ConcurrentDictionary<string, IEntityGenerator?> ServerGenerator { get; } = new();

    private ConcurrentDictionary<string, IEntityGenerator?> ClientGenerator { get; } = new();

    private ConcurrentDictionary<string, IEntityGenerator?> NeitherGenerator { get; } = new();

    public string SubcommandName => "EntityGenerator";

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

    private IEntityGenerator? GetUnknownType(string name)
    {
        return NeitherGenerator.GetOrAdd(name, (n) =>
        {
            var assembly = Assembly.GetCallingAssembly();
            var type = assembly.GetType($"Utopia.Tool.Generators.EntityGenerators.{name}", false, true);
            type ??= assembly.GetType($"Utopia.Tool.Generators.EntityGenerators.{name}Generator", false, true);

            if(type == null)
            {
                return null;
            }

            return (IEntityGenerator?)Activator.CreateInstance(type);
        });
    }

    private IEntityGenerator? GetServerType(string name)
    {
        return ServerGenerator.GetOrAdd(name, (n) =>
        {
            var assembly = Assembly.GetCallingAssembly();
            var type = assembly.GetType($"Utopia.Tool.Generators.EntityGenerators.Server.{name}", false, true);
            type ??= assembly.GetType($"Utopia.Tool.Generators.EntityGenerators.Server.{name}Generator", false, true);

            if (type == null)
            {
                return null;
            }

            return (IEntityGenerator?)Activator.CreateInstance(type);
        });
    }

    private IEntityGenerator? GetClientType(string name)
    {
        return ClientGenerator.GetOrAdd(name, (n) =>
        {
            var assembly = Assembly.GetCallingAssembly();
            var type = assembly.GetType($"Utopia.Tool.Generators.EntityGenerators.Client.{name}", false, true);
            type ??= assembly.GetType($"Utopia.Tool.Generators.EntityGenerators.Client.{name}Generator", false, true);

            if (type == null)
            {
                return null;
            }

            return (IEntityGenerator?)Activator.CreateInstance(type);
        });
    }

    private IEntityGenerator? _GetGenerator(string name,ProjectType type)
    {
        if(type == ProjectType.ShutdownEntityGenerator)
        {
            return null;
        }
        else if (type == ProjectType.Neither)
        {
            var got = GetUnknownType(name);

            if (got != null)
            {
                return got;
            }

            // check other side
            if((GetServerType(name) ?? GetClientType(name)) != null)
            {
                return null;
            }
        }
        else if(type == ProjectType.Client)
        {
            var got = GetClientType(name);

            if (got != null)
            {
                return got;
            }

            // check other side
            if ((GetServerType(name) ?? GetUnknownType(name)) != null)
            {
                return null;
            }
        }
        else if(type == ProjectType.Server)
        {
            var got = GetServerType(name);

            if (got != null)
            {
                return got;
            }

            // check other side
            if ((GetUnknownType(name) ?? GetClientType(name)) != null)
            {
                return null;
            }
        }

        // fail,not found in Server,Client or Unknown
        throw new TypeAccessException($"found no type as name:{name}");
    }

    public void Execute(GeneratorOption option)
    {
        // find all .xml
        List<string> xmlDocuments = [];

        foreach (string xmlFile in _GetAllFile(option.CurrentFileSystem.EntitiesDirectory))
        {
            if (xmlFile.EndsWith(".xml"))
            {
                xmlDocuments.Add(xmlFile);
            }
        }

        // parse
        XmlSerializer xml = new(typeof(EntityInformation));
        List<(Task, string)> tasks = [];
        foreach (string xmlDocument in xmlDocuments)
        {
            tasks.Add(new(Task.Run(() =>
            {
                try
                {
                    using FileStream fs = new(xmlDocument, FileMode.Open);
                    var obj = (EntityInformation)(xml.Deserialize(fs) ?? throw new XmlException("XmlSerializer.Deserialize return null"));

                    foreach (var generator in obj.Generators)
                    {
                        var gen = _GetGenerator(generator.GeneratorName, option.CurrentProject.Configuration.Type);

                        // skip
                        if(gen == null)
                        {
                            continue;
                        }

                        gen.Generate(xmlDocument, obj, generator.Data, option);
                    }
                }
                catch (Exception e)
                {
                    throw new AggregateException(new Exception($"Get an error when process file {xmlDocument}"), e);
                }
            }), xmlDocument));
        }

        // wait all
        try
        {
            Task.WaitAll(tasks.ToArray().Select((k) => k.Item1).ToArray());
        }
        catch (Exception e)
        {
            s_logger.Error(e, "error when generating entity");
        }
    }
}
