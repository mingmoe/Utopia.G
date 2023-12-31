using System;
using System.ComponentModel;
using System.Linq;
using Nuke.Common.Tooling;

namespace Utopia.BuildScript;

[TypeConverter(typeof(TypeConverter<Configuration>))]
public class Configuration : Enumeration
{
    public string Mode => Value;

    public static Configuration Debug = new Configuration { Value = nameof(Debug) };
    public static Configuration Release = new Configuration { Value = nameof(Release) };

    public static implicit operator string(Configuration configuration)
    {
        return configuration.Value;
    }
}
