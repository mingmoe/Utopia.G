// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Text;

namespace Utopia.Tools.Generators;

public sealed class GeneratorOption(Configuration configuration, IPluginDevFileSystem targetProject) : IDisposable
{
    public Configuration Configuration { get; set; } = configuration;

    public IPluginDevFileSystem TargetProject { get; set; } = targetProject;

    public TranslateManager TranslateManager { get; set; } = new TranslateManager(configuration, targetProject);

    public override string ToString()
    {
        var builder = new StringBuilder();

        _ = builder.AppendLine(TargetProject.ToString());
        _ = builder.AppendLine("Target Namespace:" + Configuration.RootNamespace);
        return builder.ToString();
    }

    public void Dispose()
    {
        TranslateManager.Dispose();
        GC.SuppressFinalize(this);
    }
}

