using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utopia.Tools.Generators;
public sealed class GeneratorOption : IDisposable
{
    public string TargetNamespace { get; set; }

    public IFileSystem TargetProject { get; set; }

    public TranslateManager TranslateManager { get; set; }

    public GeneratorOption(string targetNamespace, IFileSystem targetProject)
    {
        this.TargetNamespace = targetNamespace;
        this.TargetProject = targetProject;
        this.TranslateManager = new TranslateManager(targetProject);
    }

    public override string ToString()
    {
        var builder = new StringBuilder();

        builder.AppendLine(this.TargetProject.ToString());
        builder.AppendLine("Target Namespace:" + this.TargetNamespace);
        return builder.ToString();
    }

    public void Dispose()
    {
        this.TranslateManager.Dispose();
        GC.SuppressFinalize(this);
    }
}

