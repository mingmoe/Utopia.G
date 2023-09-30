#region copyright
// This file(may named IPlayer.cs) is a part of the project: Utopia.Server.
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

using CommunityToolkit.Diagnostics;
using Utopia.Core.Map;
using Utopia.Core.Translate;
using Utopia.Core.Utilities;
using Utopia.G.Net;

namespace Utopia.Server.Net;

public interface IPlayer : Map.IEntity
{
    /// <summary>
    /// 玩家位置
    /// </summary>
    WorldPosition PlayerPosition { get; }

    /// <summary>
    /// 玩家的客户端
    /// </summary>
    IConnectHandler Client { get; }
}

public class Player : IPlayer
{
    public static readonly Guuid PlayerEntityId = new("utopia", "core", "entity", "player");

    public WorldPosition PlayerPosition { get; set; }

    public IConnectHandler Client { get; init; }

    public TranslateKey Name { get; init; }

    public bool Accessible => false;

    public bool Collidable => true;

    public Guuid Id => PlayerEntityId;

    public WorldPosition WorldPosition { get; set; }

    public void LogicUpdate()
    {

    }

    public byte[] Save()
    {
        throw new NotImplementedException();
    }

    public Player(IConnectHandler client, string playerId)
    {
        Guard.IsNotNull(client);
        this.Client = client;
        this.Name = TranslateKey.Create(playerId, "player id");
    }
}

