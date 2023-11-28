// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.Xml.Serialization;
using SharpCompress.Writers;

namespace Utopia.Core;

public static class Xml
{
    /// <summary>
    /// For utopia
    /// </summary>
    public const string Namespace = "http://utopia.kawayi.moe";

    public static XmlSchemas GetXmlSchema<T>()
    {
        return GetXmlSchema(typeof(T));
    }

    public static XmlSchemas GetXmlSchema(Type t)
    {
        XmlSchemas schemas = [];
        XmlSchemaExporter exporter = new(schemas);

        XmlTypeMapping mapping = new XmlReflectionImporter().ImportTypeMapping(t);
        exporter.ExportTypeMapping(mapping);

        return schemas;
    }

    public static void WriteXmlSchemas(XmlSchemas schemas,Stream output)
    {
        foreach (XmlSchema schema in schemas)
        {
            schema.Write(output);
        }
    }
}
