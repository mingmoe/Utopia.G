// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Utopia.Core.Configuration;
using Utopia.Core.IO;
using Utopia.Core.Translation;

namespace Utopia.Core.Plugin;

/// <summary>
/// A tool class for loading plugins
/// </summary>
public class PluginHelper<PluginT> where PluginT : IPluginInformation, IPluginBase
{
    public required LanguageID CurrentLanguage { get; init; }

    public required IFileSystem FileSystem { get; init; }

    public required IContainer Container { get; init; }

    public IEnumerable<PluginContext<PluginT>> LoadPackedPlugin(string packetPluginFile)
    {
        var pluginRoot = FileSystem.GetExtractedDirectoryOfPacketPlugin(packetPluginFile);
        var manifest = ExtractPackedPlugin(
            packetPluginFile,
            FileSystem.GetExtractedPluginLockFileOfPacketPlugin(packetPluginFile),
            pluginRoot);
        List<PluginContext<PluginT>> plugins = [];

        // for every assembly
        foreach (var assembly in manifest.Assemblies)
        {
            var assemblyFile = Path.Join(pluginRoot, assembly);

            var loaded = Assembly.LoadFrom(assemblyFile);

            // look up for plugin
            foreach (var exported in loaded.ExportedTypes)
            {
                if (!exported.IsAssignableTo<PluginT>())
                {
                    continue;
                }

                // get it
                var context = BuildPluginFromType(
                    exported,
                    pluginRoot,
                    packetPluginFile,
                    assemblyFile,
                    manifest);

                plugins.Add(context);
            }
        }

        return plugins;
    }

    public PluginContext<PluginT> BuildPluginFromType(
        Type plugin,
        string pluginRoot,
        string? packedPluginFile,
        string? assemblyFile,
        PackedPluginManifest manifest)
    {
        MethodInfo[] methods = plugin.GetMethods(
                    BindingFlags.Public
                    | BindingFlags.Static)
                    .TakeWhile((method) => method.GetCustomAttribute<ContainerBuildAttribute>() != null)
                    .ToArray();

        var container =
            Container.BeginLifetimeScope(
                (builder) => {
                    BuildPluginContainer(
                        builder,
                        plugin,
                        pluginRoot,
                        packedPluginFile,
                        assemblyFile,
                        manifest);

                    // call build methods
                    foreach (var method in methods)
                    {
                        method.Invoke(null, [builder]);
                    }
                });

        try
        {
            return container.Resolve<PluginContext<PluginT>>();
        }
        catch (Exception)
        {
            // avoid leaking
            container.Dispose();
            throw;
        }
    }

    public IEnumerable<PluginContext<PluginT>> LoadAllPackedPluginsFromDirectory(string directory)
    {
        List<PluginContext<PluginT>> plugins = [];
        foreach (var file in Directory.GetFiles(directory,"*.*", SearchOption.AllDirectories))
        {
            plugins.AddRange(LoadPackedPlugin(file));
        }
        return plugins;
    }

    public IPluginFileSystem CreatePluginFileSystem(
        string pluginRoot,
        string? packedPluginFile,
        string? assemblyFile,
        string configurationDirectory)
    {
        return new PluginFileSystem(
                    pluginRoot,
                    packedPluginFile,
                    assemblyFile,
                    configurationDirectory
                );
    }

    public void BuildPluginContainer(
        ContainerBuilder builder,
        Type pluginType,
        string pluginRoot,
        string? packetPluginFile,
        string? assemblyFile,
        PackedPluginManifest manifest)
    {
        // register PluginT && IPluginFileSystem
        builder
            .RegisterType(pluginType)
            .SingleInstance()
            .As<PluginT>();

        builder
            .Register((c) =>
            {
                var @class = c.Resolve<PluginT>();

                return CreatePluginFileSystem(
                                        pluginRoot,
                                        packetPluginFile,
                                        assemblyFile,
                                        FileSystem.GetConfigurationDirectoryOfPlugin(@class));
            })
            .SingleInstance()
            .As<IPluginFileSystem>();

        // IConfiguration
        builder
            .RegisterType<ConfigurationLoader>()
            .SingleInstance()
            .As<IConfigurationLoader>();

        // Manifest
        builder
            .RegisterInstance(manifest)
            .SingleInstance();

        // ILifetimeScope was registered automatically

        // PluginContext
        builder
            .RegisterType<PluginContext<PluginT>>()
            .SingleInstance()
            .AsSelf();

        // ITranslationManager
        builder
            .RegisterType<TranslationManager>()
            .SingleInstance()
            .As<ITranslationManager>();

        // LanguageID
        builder
            .RegisterInstance(CurrentLanguage)
            .SingleInstance()
            .AsSelf();

        // ITranslationGetter
        builder
            .RegisterType<TranslationGetter>()
            .SingleInstance()
            .As<ITranslationGetter>();
    }

    public PluginContext<PluginT> CreatePluginContext(
        PluginT plugin,
        PackedPluginManifest manifest,
        ILifetimeScope lifetimeScope,
        IPluginFileSystem fileSystem
        )
    {
        return new PluginContext<PluginT>(
                fileSystem,
                plugin,
                manifest,
                lifetimeScope
                );
    }

    /// <summary>
    /// Extract a packed plugin file
    /// </summary>
    /// <param name="packetPluginFile">the path to the packed plugin file</param>
    /// <param name="forceReExtract">if true,extract the contents of the packed file to output directory,
    /// which from <see cref="IFileSystem.GetExtractedDirectoryOfPacketPlugin(string)"/></param>
    /// <returns>the manifest extracted from the packed plugin file</returns>
    public PackedPluginManifest ExtractPackedPlugin(
        string packetPluginFile,
        string? lockFile,
        string outputDirectory,
        bool forceReExtract = false)
    {
        byte[]? md5 = null;

        if (File.Exists(lockFile))
        {
            md5 = File.ReadAllBytes(lockFile);
        }

        var realMd5 = FileUtilities.GetFileMd5(packetPluginFile);

        if (lockFile is not null)
        {
            File.WriteAllBytes(lockFile, realMd5);
        }

        if (md5 == null || !Enumerable.SequenceEqual(md5, realMd5))
        {
            forceReExtract = true;
        }

        using var fs = File.OpenRead(packetPluginFile);

        return PackedPlugin.ReadPack(fs, forceReExtract ? outputDirectory : null);
    }
}
