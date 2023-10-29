// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.Threading.Tasks;
using Godot;
using Utopia.Core;
using Utopia.Core.Events;
using Utopia.Core.Net.Packet;
using Utopia.Core.Plugin;
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

    public PluginLifeCycle CurrentCycle => throw new NotImplementedException();

    public IEventManager<LifeCycleEvent<PluginLifeCycle>> LifecycleEvent => throw new NotImplementedException();

    public CorePlugin(Core.IServiceProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider);
        _Provider = provider;
    }

    public void Activate()
    {
        IEntityManager manager = _Provider.GetService<IEntityManager>();
        var factory = new EmptyEntityFactory();
        Node node = _Provider.GetService<Node>();

        Texture2D grass = ResourceLoader.Load<Texture2D>("res://images/textures/grass.png");

        var source = new TileSetAtlasSource
        {
            TextureRegionSize = new(32, 32),
            Texture = grass
        };
        int id = TileSource.CreateSingleTile(source, grass);

        _ = ((Main)node).Map.TileSet.AddSource(source, 1);

        _ = factory.Entities.TryAdd(ResourcePack.Entity.GrassEntity.ID,
            new GrassEntity(new Tile((int)TileLayer.Floor, 1, id)));

        _ = manager.TryAdd(
                ResourcePack.Entity.GrassEntity.ID,
                factory
            );

        _Provider.GetService<IEventBus>().Register<LifeCycleEvent<LifeCycle>>((e) =>
        {
            if (e.Order == LifeCycleOrder.After && e.Cycle == LifeCycle.ConnectToServer)
            {
                // process
                ISocketConnecter connecter = _Provider.GetService<ISocketConnecter>();

                connecter.ConnectHandler!.Packetizer.EnterSync(
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

                        for (int index = 0; index != pack.Entities.Length; index++)
                        {
                            Core.Utilities.Guuid entity = pack.Entities[index];
                            byte[] data = pack.EntityData[index];

                            IGodotEntity got = manager.Create(entity, data);

                            Node? tile = got.Render(pack.Position, ((Main)node).Map);

                            node.AddChild(tile);
                        }
                    });

                _ = Task.Run(() =>
                {
                    for (int x = -32; x != 32; x++)
                    {
                        for (int y = -32; y != 32; y++)
                        {
                            connecter.ConnectHandler!.WritePacket(QueryBlockPacketFormatter.PacketTypeId,
                                new QueryBlockPacket() { QueryPosition = new Core.Map.WorldPosition(x, y, 0, 0) });
                        }
                    }
                });
            }
        });
    }
    public void Deactivate() => throw new NotImplementedException();
}
