using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utopia.Core.Configuration;

[AttributeUsage(AttributeTargets.Class)]
public class ConfigurationAttribute : Attribute
{
    public readonly string FileName;

    public ConfigurationAttribute(string fileName)
    {
        this.FileName = fileName;
    }
}

