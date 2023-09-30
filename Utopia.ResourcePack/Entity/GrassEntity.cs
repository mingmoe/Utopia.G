#region copyright
// This file(may named GrassEntity.cs) is a part of the project: Utopia.ResourcePack.
// 
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// 
// This file is part of Utopia.ResourcePack.
//
// Utopia.ResourcePack is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Utopia.ResourcePack is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with Utopia.ResourcePack. If not, see <https://www.gnu.org/licenses/>.
#endregion

using Utopia.Core.Translate;
using Utopia.Core.Utilities;

namespace Utopia.ResourcePack.Entity;

public static class GrassEntity
{
    public static readonly Guuid ID = new("utopia", "core", "entity", "grass");

    public static readonly TranslateKey TranslateKey = new(
        "utopia:core:translate:entity:grass", "the name of the entity `grass`");
}
