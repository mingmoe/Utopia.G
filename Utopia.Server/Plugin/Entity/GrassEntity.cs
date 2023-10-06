#region copyright
// This file(may named GrassEntity.cs) is a part of the project: Utopia.Server.
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

using Utopia.Core.Map;
using Utopia.Core.Translate;
using Utopia.Core.Utilities;
using Utopia.Server.Map;

namespace Utopia.Server.Plugin.Entity;

/// <summary>
/// 草地实体，曾经一段时间被用于测试
/// </summary>
public class GrassEntity : IEntity
{
    public ITranslatedString Name { get; init; }

    public bool Accessible => false;

    public bool Collidable => false;

    public Guuid Id => ResourcePack.Entity.GrassEntity.ID;

    public WorldPosition WorldPosition { get; set; }

    public GrassEntity(ITranslateManager msg, TranslateIdentifence id)
    {
        //        this.Name = new ICUTranslatedString(ResourcePack.Entity.GrassEntity.TranslateKey, msg, id, new object());
        this.WorldPosition = new WorldPosition(1, 1, 1, 1);
        this.Name = new TranslatedString("translated");
    }

    public void LogicUpdate()
    {

    }

    public byte[] Save()
    {
        return Array.Empty<byte>();
    }

    public byte[] ClientOnlyData()
    {
        return Array.Empty<byte>();
    }
}
