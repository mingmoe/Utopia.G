// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Collections.Concurrent;
using NLog;
using Tomlyn;
using Tomlyn.Model;

namespace Utopia.Tools.Generators;

/// <summary>
/// The entity information of the generated.
/// </summary>
public class GeneratedEntityInfo
{
    public bool Collidable { get; set; }

    public bool Accessible { get; set; }

    public string Guuid { get; set; } = null!;

    /// <summary>
    /// This hint use which entity generator.
    /// </summary>
    public string Type { get; set; } = null!;

    public TomlTable Data { get; set; } = null!;
}

/// <summary>
/// This generator will generate the entity class.
/// Just because it's a hard job. So we need many other entity generators to generate diverse entity.
/// We also need multiple threads.
/// </summary>
public class EntityGenerator : IGenerator
{

    private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

    public int ThreadCount { get; set; }

    public ConcurrentDictionary<string, IEntityGenerator> Generators { get; } = new();

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
        // find all .toml
        List<string> tomls = new();

        foreach (string toml in _GetAllFile(option.TargetProject.EntitiesDirectory))
        {
            if (toml.EndsWith(".toml"))
            {
                tomls.Add(toml);
            }
        }

        // parse
        foreach (string toml in tomls)
        {
            try
            {
                GeneratedEntityInfo info = Toml.ToModel<GeneratedEntityInfo>(toml);

                // find type
                string type = info.Type;

                if (Generators.TryGetValue(type, out IEntityGenerator? generator))
                {
                    generator!.Generate(toml, info, option);
                }
                else
                {
                    throw new InvalidOperationException("failed to find the generator for the file");
                }
            }
            catch (Exception)
            {
                s_logger.Error("get a exception when process file:{toml}", toml);
                throw;
            }
        }
    }
}
