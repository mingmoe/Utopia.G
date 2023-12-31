// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Utopia.Core.IO;

namespace Utopia.Core.Plugin;

/// <summary>
/// Stands for a plugin, including its instance, filesystem ...
/// </summary>
public sealed class PluginContext<PluginT>(
    IPluginFileSystem fileSystem,
    PluginT instance,
    PackedPluginManifest manifest,
    ILifetimeScope lifetimeScope) : IDisposable where PluginT : IPluginInformation, IPluginBase
{
    private bool _disposed = false;

    public IPluginFileSystem FileSystem { get; init; } = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));

    public PluginT Instance { get; } = instance ?? throw new ArgumentNullException(nameof(instance));

    public PackedPluginManifest Manifest { get; init; } = manifest ?? throw new ArgumentNullException(nameof(manifest));

    public ILifetimeScope LifetimeScope { get; init; } = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            Instance.Deactivate();
            LifetimeScope.Dispose();
        }

        _disposed = true;
    }
}