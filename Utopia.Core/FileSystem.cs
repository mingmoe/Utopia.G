//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//

namespace Utopia.Core;

/// <summary>
/// 文件系统
/// </summary>
public abstract class FileSystem : IFileSystem
{
    public abstract string Root { get; }

    public string Asserts => Path.Join(this.Root, "Asserts");

    public string Worlds => Path.Join(this.Root, "Worlds");

    public string Characters => Path.Join(this.Root, "Characters");

    public string Plugins => Path.Join(this.Root, "Plugins");

    public string Configuraions => Path.Join(this.Root, "Configurations");

    public string Utilties => Path.Join(this.Root, "Utilties");

    public abstract string? Server { get; }
}
