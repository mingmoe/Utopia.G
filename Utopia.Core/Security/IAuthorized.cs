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
/// This class stands for a account.
/// </summary>
public interface IAuthorized
{
    /// <summary>
    /// The unique user id.
    /// Note: this was the key to separate different users.
    /// </summary>
    public string UserId { get; }

    /// <summary>
    /// The user name.
    /// This usually can be changed and can be repetitive for multiple user(with different <see cref="UserId"/>).
    /// </summary>
    public string UserName { get; }

    /// <summary>
    /// The way the user login
    /// </summary>
    public string? LoginFrom { get; }

    /// <summary>
    /// The user email address.
    /// </summary>
    public string? UserEmail { get; }
}
