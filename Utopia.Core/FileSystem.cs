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

namespace Utopia.Core
{
    /// <summary>
    /// 文件系统
    /// </summary>
    public abstract class FileSystem : IFileSystem
    {
        public abstract string Root { get; }

        public string Asserts => Path.Join(Root, "asserts");

        public string Worlds => Path.Join(Root, "worlds");

        public string Characters => Path.Join(Root, "characters");

        public string Plugins => Path.Join(Root, "plugins");

        public string Configuraions => Path.Join(Root, "configurations");
    }
}
