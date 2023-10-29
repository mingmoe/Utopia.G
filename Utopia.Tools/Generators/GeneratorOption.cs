// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Text;

namespace Utopia.Tools.Generators;
public sealed class GeneratorOption : IDisposable
{
    public string TargetNamespace { get; set; }

    public IPluginDevFileSystem TargetProject { get; set; }

    public TranslateManager TranslateManager { get; set; }

    public GeneratorOption(string targetNamespace, IPluginDevFileSystem targetProject)
    {
        TargetNamespace = targetNamespace;
        TargetProject = targetProject;
        TranslateManager = new TranslateManager(targetProject);
    }

    public override string ToString()
    {
        var builder = new StringBuilder();

        _ = builder.AppendLine(TargetProject.ToString());
        _ = builder.AppendLine("Target Namespace:" + TargetNamespace);
        return builder.ToString();
    }

    public void Dispose()
    {
        TranslateManager.Dispose();
        GC.SuppressFinalize(this);
    }
}

