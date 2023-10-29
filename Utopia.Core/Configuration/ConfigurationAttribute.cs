// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

namespace Utopia.Core.Configuration;

[AttributeUsage(AttributeTargets.Class)]
public class ConfigurationAttribute : Attribute
{
    public readonly string FileName;

    public ConfigurationAttribute(string fileName) => FileName = fileName;
}

