// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using HarmonyLib;
using SharpCompress.Archives;
using SharpCompress.Archives.GZip;
using SharpCompress.Archives.Tar;
using SharpCompress.Common;
using SharpCompress.Readers;
using SharpCompress.Readers.Tar;
using SharpCompress.Writers;
using SharpCompress.Writers.GZip;
using SharpCompress.Writers.Tar;

namespace Utopia.Core.Plugin;

public class PackedPluginManifest
{
    /// <summary>
    /// which assembly should be loaded.
    /// only the assembly where the <see cref="IPluginBase"/> is,
    /// dependencies will be loaded automatically.
    /// should be found in the <see cref="PackedPlugin.Contents"/>.
    /// </summary>
    [XmlArray("Assemblies")]
    [XmlArrayItem("Assembly")]
    public string[] Assemblies { get; set; } = [];
}

public class PackedPlugin
{
    /// <summary>
    /// the contents of the plugin
    /// </summary>
    [XmlArray("Contents")]
    [XmlArrayItem("Content")]
    public string[] Contents { get; set; } = [];

    /// <summary>
    /// the manifest of the plugin,some meta information
    /// </summary>
    [XmlElement]
    public PackedPluginManifest Manifest { get; set; } = new();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="outputStream"></param>
    /// <param name="leaveStreamOpen">if true,keep the stream open when return</param>
    /// <exception cref="IOException"></exception>
    public void CreatePack(Stream outputStream,bool leaveStreamOpen)
    {
        // pack files
        using var tar = new TarWriter
            (outputStream,new TarWriterOptions(CompressionType.None, true)
            {
                ArchiveEncoding = new ArchiveEncoding(Encoding.UTF8, Encoding.UTF8),
                LeaveStreamOpen = leaveStreamOpen
            });

        // write manifest
        using (var memory = new MemoryStream())
        {
            XmlSerializer xml = new(typeof(PackedPluginManifest));

            xml.Serialize(memory, Manifest);

            tar.Write(nameof(Manifest), memory);
        }

        // compress contents
        using (var memory = new MemoryStream())
        {
            using (var contents = new TarWriter(
                memory, new TarWriterOptions(CompressionType.Deflate64, true)
                {
                    ArchiveEncoding = new ArchiveEncoding(Encoding.UTF8, Encoding.UTF8),
                }))
            {
                foreach (var content in Contents)
                {
                    if (File.Exists(content))
                    {
                        using var fs = File.OpenRead(content);
                        contents.Write(Path.GetFileName(content), fs);
                    }
                    else if (Directory.Exists(content))
                    {
                        contents.WriteAll(content, fileSearchFunc: null);
                    }
                    else
                    {
                        throw new IOException($"Invalid path:{content}");
                    }
                }
            }
            tar.Write(nameof(Contents), memory);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="inputStream"></param>
    /// <param name="outputDirectory">if this is null,not extract contents.
    /// otherwise extract contents to the directory</param>
    /// <returns></returns>
    /// <exception cref="FormatException"></exception>
    /// <exception cref="XmlException"></exception>
    public static PackedPluginManifest
        ReadPack(Stream inputStream, string? outputDirectory)
    {
        var tar = TarArchive.Open(inputStream,new()
        {
            ArchiveEncoding = new ArchiveEncoding(Encoding.UTF8, Encoding.UTF8),
        });

        var items = tar.Entries.ToDictionary((e) => e.Key);
        if (!(items.Keys.ToArray().Length == 2 &&
            items.ContainsKey(nameof(Manifest)) &&
            items.ContainsKey(nameof(Contents))))
        {
            throw new FormatException("the packed plugin was broken");
        }

        // read manifest
        PackedPluginManifest manifest;
        using (var memory = new MemoryStream())
        {
            XmlSerializer xml = new(typeof(PackedPluginManifest));

            items[nameof(Manifest)].Archive.ExtractAllEntries().WriteEntryTo(memory);

            manifest = (PackedPluginManifest?)xml.Deserialize(memory)
                ?? throw new XmlException("XmlSerializer.Deserialize() returns null");
        }

        // get contents
        if (outputDirectory is not null)
        {
            TarReader contentReader = TarReader.Open(items[nameof(Contents)].OpenEntryStream(), new()
            {
                ArchiveEncoding = new(Encoding.UTF8, Encoding.UTF8),
                LeaveStreamOpen = true,
            });

            contentReader.WriteEntryToDirectory(outputDirectory);
        }

        return manifest;
    }
}
