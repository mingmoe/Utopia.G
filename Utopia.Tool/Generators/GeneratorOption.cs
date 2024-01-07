// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Text;
using Utopia.Tool;

namespace Utopia.Tool.Generators;

public sealed class GeneratorOption(
    Configuration globalConfiguration,
    SubprojectConfiguration currentProject,
    IPluginDevFileSystem currentFileSystem,
    TranslateManager translate) : IDisposable
{
    public Configuration Configuration { get; set; } = globalConfiguration;

    public SubprojectConfiguration CurrentProject { get; set; } = currentProject;

    public IPluginDevFileSystem CurrentFileSystem { get; set; } = currentFileSystem;

    public TranslateManager TranslateManager { get; set; } = translate;

    public override string ToString()
    {
        var builder = new StringBuilder();

        _ = builder.AppendLine(CurrentFileSystem.ToString());
        return builder.ToString();
    }

    public void Dispose()
    {
        TranslateManager.Dispose();
        GC.SuppressFinalize(this);
    }
}

