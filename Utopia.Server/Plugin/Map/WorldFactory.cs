#region copyright
// This file(may named WorldFactory.cs) is a part of the project: Utopia.Server.
// 
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// 
// This file is part of Utopia.Server.
//
// Utopia.Server is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Utopia.Server is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with Utopia.Server. If not, see <https://www.gnu.org/licenses/>.
#endregion

using Utopia.Core.Utilities;
using Utopia.Server.Map;

namespace Utopia.Server.Plugin.Map;

/// <summary>
/// 世界工厂
/// </summary>
public class WorldFactory : IWorldFactory
{
    public Guuid WorldType => IDs.WorldType;

    private readonly Generator _generator;

    public WorldFactory(Generator generator)
    {
        ArgumentNullException.ThrowIfNull(generator, nameof(generator));
        this._generator = generator;
    }

    public IWorld GenerateNewWorld()
    {
        return new World(0, 4, 4, this._generator);
    }
}
