// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using Utopia.Core.Utilities;

namespace Utopia.Core.Net;

/// <summary>
/// 包格式化器
/// </summary>
public interface IPacketFormatter
{
    Guuid Id { get; }

    object GetValue(byte[] packet);

    byte[] ToPacket(object value);
}
