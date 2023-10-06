#region copyright
// This file(may named CorePlugin.cs) is a part of the project: Utopia.G.
// 
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// 
// This file is part of Utopia.G.
//
// Utopia.G is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Utopia.G is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with Utopia.G. If not, see <https://www.gnu.org/licenses/>.
#endregion

using Godot;
using System;
using System.Threading.Tasks;
using Utopia.Core;
using Utopia.Core.Events;
using Utopia.Core.Net.Packet;
using Utopia.G.Game;
using Utopia.G.Game.Entity;
using Utopia.G.Graphy;
using Utopia.G.Net;
using Utopia.G.Plugin.Entity;
using Utopia.ResourcePack;

namespace Utopia.G.Plugin;

/// <summary>
/// 核心插件
/// </summary>
public class CorePlugin : PluginInformation, IPlugin
{
    private Core.IServiceProvider _Provider { get; init; }

    public CorePlugin(Core.IServiceProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider);
        this._Provider = provider;
    }

    public void Active()
    {
        var manager = this._Provider.GetService<IEntityManager>();
        var factory = new EmptyEntityFactory();
        var node = this._Provider.GetService<Node>();

        var grass = ResourceLoader.Load<Texture2D>("res://images/textures/grass.png");

        var source = new TileSetAtlasSource
        {
            TextureRegionSize = new(32, 32),
            Texture = grass
        };
        var id = TileSource.CreateSingleTile(source, grass);

        ((Main)node).Map.TileSet.AddSource(source, 1);

        factory.Entities.TryAdd(ResourcePack.Entity.GrassEntity.ID,
            new GrassEntity(new Tile((int)TileLayer.Floor, 1, id)));

        manager.TryAdd(
                ResourcePack.Entity.GrassEntity.ID,
                factory
            );

        this._Provider.GetService<IEventBus>().Register<LifeCycleEvent<LifeCycle>>((e) =>
        {
            if (e.Order == LifeCycleOrder.After && e.Cycle == LifeCycle.ConnectToServer)
            {
                // process
                var connecter = this._Provider.GetService<ISocketConnecter>();

                connecter.ConnectHandler!.Packetizer.OperateFormatterList(
                    (list) =>
                    {
                        list.Add(new LoginPacketFormatter());
                        list.Add(new BlockInfoPacketFormatter());
                        list.Add(new QueryBlockPacketFormatter());
                    });

                connecter.ConnectHandler!.Dispatcher.RegisterHandler(
                    BlockInfoPacketFormatter.PacketTypeId, (packet) =>
                    {
                        var pack = (BlockInfoPacket)packet;

                        for (var index = 0; index != pack.Entities.Length; index++)
                        {
                            var entity = pack.Entities[index];
                            var data = pack.EntityData[index];

                            var got = manager.Create(entity, data);

                            var tile = got.Render(pack.Position, ((Main)node).Map);

                            node.AddChild(tile);
                        }
                    });

                Task.Run(() =>
                {
                    for (var x = -32; x != 32; x++)
                    {
                        for (var y = -32; y != 32; y++)
                        {
                            connecter.ConnectHandler!.WritePacket(QueryBlockPacketFormatter.PacketTypeId,
                                new QueryBlockPacket() { QueryPosition = new Core.Map.WorldPosition(x, y, 0, 0) });
                        }
                    }
                });
            }
        });
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
