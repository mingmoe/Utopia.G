using Microsoft.AspNetCore.Razor.TagHelpers;
using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tomlyn;
using Tomlyn.Model;

namespace Utopia.Tools.Generators;

public class EntityInfo
{
    public bool Collidable { get; set; }

    public bool Accessible { get; set; }

    public string Guuid { get; set; }

    /// <summary>
    /// This hint use which entity generator.
    /// </summary>
    public string Type { get; set; }

    public TomlTable Data { get; set; }
}

/// <summary>
/// This generator will generate the entity class.
/// Just because it's a hard job. So we need many other entity generators to generate diverse entity.
/// We also need multiple threads.
/// </summary>
public class EntityGenerator : IGenerator
{

    private readonly static Logger _logger = LogManager.GetCurrentClassLogger();

    public int ThreadCount { get; set; }

    public ConcurrentDictionary<string, IEntityGenerator> Generators { get; } = new();

    private static IEnumerable<string> _GetAllFile(string dir)
    {
        dir = Path.GetFullPath(dir);
        foreach (var f in Directory.GetFiles(dir))
        {
            yield return f;
        }
        foreach (string d in Directory.GetDirectories(dir))
        {
            foreach(var f in _GetAllFile(d))
            {
                yield return f;
            }
        }
    }

    public void Execute(GeneratorOption option)
    {
        // find all .toml
        List<string> tomls = new();

        foreach(var toml in _GetAllFile(option.TargetProject.EntitiesDir))
        {
            if (toml.EndsWith(".toml"))
            {
                tomls.Add(toml);
            }
        }
        
        // parse
        foreach(var toml in tomls)
        {
            try
            {
                var info = Toml.ToModel<EntityInfo>(toml);

                // find type
                var type = info.Type;

                if (this.Generators.TryGetValue(type, out IEntityGenerator? generator))
                {
                    generator!.Generate(toml, info, option);
                }
                else
                {
                    throw new InvalidOperationException("failed to find the generator for the file");
                }
            }
            catch(Exception)
            {
                _logger.Error("get a exception when process file:{toml}", toml);
                throw;
            }
        }
    }
}
