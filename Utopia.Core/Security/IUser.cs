// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utopia.Core.Security;
/// <summary>
/// This class stands for a player.
/// </summary>
public interface IUser
{
    /// <summary>
    /// The login information of the user.
    /// </summary>
    public IAuthorized Authentication { get; set; }

    /// <summary>
    /// The permission of the user.
    /// </summary>
    public PermissionManager PermissionManager { get; set; }
}
