// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utopia.Core;

/// <summary>
/// 
/// </summary>
public static class VersionUtility
{
    private static Lazy<Version> _version = new(() =>
    {
        return typeof(VersionUtility).Assembly.GetName().Version ?? new Version(0, 1, 0, 0);
    }, true);

    public static Version UtopiaCoreVersion { get {
            return _version.Value;
        }
    }
}
