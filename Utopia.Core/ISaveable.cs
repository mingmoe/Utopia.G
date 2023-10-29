// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

namespace Utopia.Core;

/// <summary>
/// This interface stands for a class that can save itself as bytes.
/// So that the class can be saved to file or be transmitted in the Internet.
/// </summary>
public interface ISaveable
{

    byte[] Save();

}

/// <summary>
/// Like <see cref="ISaveable"/> but it wont save as bytes.
/// It will save itself to some other object.
/// May be a <see cref="Stream"/> or a <see cref="DirectoryInfo"/> or a <see cref="FileInfo"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ISaveableTo<T>
{
    void SaveTo(T obj);
}
