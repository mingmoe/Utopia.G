using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utopia.Tools.Generators;
public sealed class GeneratorOption
{
    public string TargetNamespace { get; set; }

    public IFileSystem TargetProject { get; set; }

    public GeneratorOption(string targetNamespace, IFileSystem targetProject)
    {
        this.TargetNamespace = targetNamespace;
        this.TargetProject = targetProject;
    }
}

