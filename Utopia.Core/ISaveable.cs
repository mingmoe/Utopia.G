#region copyright
// This file(may named ISaveable.cs) is a part of the project: Utopia.Core.
// 
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// 
// This file is part of Utopia.Core.
//
// Utopia.Core is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Utopia.Core is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with Utopia.Core. If not, see <https://www.gnu.org/licenses/>.
#endregion

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