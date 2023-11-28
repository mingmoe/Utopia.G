// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utopia.Core.IO;

namespace Utopia.Core.Configuration;

/// <summary>
/// This configuration usually using <see cref="IPluginFileSystem"/> to read.
/// </summary>
public interface IConfigurationLoader
{
    /// <summary>
    /// Load configuration,if path is null,
    /// read path from <see cref="PluginConfigurationAttribute"/> from type.
    /// </summary>
    T Load<T>(string? path = null);

    /// <summary>
    /// Store configuration,if path is null,
    /// read path from <see cref="PluginConfigurationAttribute"/> from type.
    /// </summary>
    void Store<T>(T configuration,string? path = null);
}
