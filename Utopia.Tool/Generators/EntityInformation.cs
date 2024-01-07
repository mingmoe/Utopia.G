// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using NLog;
using Utopia.Core.Utilities;

namespace Utopia.Tool.Generators;

[XmlRoot("Entity",Namespace = Xml.Namespace)]
public class EntityInformation
{
    public class Generator
    {
        [XmlElement]
        public string GeneratorName { get; set; } = string.Empty;

        [XmlElement]
        public XmlElement Data { get; set; } = null!;
    }

    [XmlElement]
    public bool Collidable { get; set; }

    [XmlElement]
    public bool Accessible { get; set; }

    [XmlElement]
    public XmlGuuid EntityId { get; set; } = new();

    [XmlElement]
    public string EntityName { get; set; } = string.Empty;

    [XmlElement]
    public string EntityDescription { get; set; } = string.Empty;

    /// <summary>
    /// This hint use which entity generator.
    /// </summary>
    [XmlArray("Generators")]
    [XmlArrayItem("Generator")]
    public Generator[] Generators { get; set; } = [];
}
