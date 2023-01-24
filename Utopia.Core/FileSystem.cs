//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Utopia.Core;

/// <summary>
/// 文件系统
/// </summary>
public abstract class FileSystem : IFileSystem
{
    public abstract string Root { get; }

    public string Asserts => Path.Join(this.Root, "asserts");

    public string Worlds => Path.Join(this.Root, "worlds");

    public string Characters => Path.Join(this.Root, "characters");

    public string Plugins => Path.Join(this.Root, "plugins");

    public string Configuraions => Path.Join(this.Root, "configurations");
}
