// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using static Utopia.Tools.Generators.IPluginDevFileSystem;

namespace Utopia.Tools.Generators.Server;

public interface IServerEntityGenerator
{
    /// <summary>
    /// Generate the entity.
    /// </summary>
    /// <param name="sourcePath">The file path which toggle the generation</param>
    /// <param name="info">The parsed entity information from the entity file</param>
    /// <param name="option">The generator options.</param>
    /// <param name="entityPath">The target directory to output the source files.</param>
    void Generate(string sourcePath, ServerEntityInfo info, GeneratorOption option);
}
