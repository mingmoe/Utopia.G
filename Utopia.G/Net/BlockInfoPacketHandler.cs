// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;
using Utopia.Core.Net;
using Utopia.Core.Net.Packet;
using Utopia.Core.Utilities;
using Utopia.G.Game.Entity;

namespace Utopia.G.Net;
public class BlockInfoPacketHandler : IPacketHandler
{
    public required Node Node { protected get; init; }

    public required IEntityManager EntityManager { protected get; init; }

    public Task Handle(Guuid packetId, object packet)
    {
        var pack = (BlockInfoPacket)packet;

        for (int index = 0; index != pack.Entities.Length; index++)
        {
            Core.Utilities.Guuid entity = pack.Entities[index];
            byte[] data = pack.EntityData[index];
            IGodotEntity got = EntityManager.Create(entity, data);

            Node? tile = got.Render(pack.Position, ((Main)Node).Map);

            Node.AddChild(tile);
        }

        return Task.FromResult(0);
    }
}
