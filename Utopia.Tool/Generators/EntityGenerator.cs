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

    public ConcurrentDictionary<string, IEntityGenerator> CustomGenerators { get; } = new();

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

    private IEntityGenerator _GetGenerator(string name)
    {
        return CustomGenerators.GetOrAdd(name, (name) =>
        {
            return (IEntityGenerator?)(Activator.CreateInstance(null!, name)?.Unwrap())
            ?? throw new ArgumentException("Unknown IEntityGenerator name");
        });
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
                        var gen = _GetGenerator(generator.GeneratorName);

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
