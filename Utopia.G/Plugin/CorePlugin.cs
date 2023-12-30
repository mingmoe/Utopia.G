// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core.Lifetime;
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

    public required IEntityManager EntityManager { get; init; }

    public required IEventBus EventBus { get; init; }

    public required ISocketConnecter SocketConnecter { get; init; }

    public required Node Node { get; init; }

    public required ILifetimeScope LifetimeScope { private get; init; }

    public PluginLifeCycle CurrentCycle => throw new NotImplementedException();

    public IEventManager<LifeCycleEvent<PluginLifeCycle>> LifecycleEvent => throw new NotImplementedException();

    public void Activate()
    {
        var factory = new EmptyEntityFactory();
        Node node = Node;

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

        _ = EntityManager.TryAdd(
                ResourcePack.Entity.GrassEntity.ID,
                factory
            );

        EventBus.Register<LifeCycleEvent<LifeCycle>>((e) =>
        {
            if (e.Order == LifeCycleOrder.After && e.Cycle == LifeCycle.ConnectToServer)
            {
                // process
                ISocketConnecter connecter = SocketConnecter;

                var container = LifetimeScope.BeginLifetimeScope((builder) =>
                {
                    builder.RegisterInstance(connecter);
                    builder.RegisterType<BlockInfoPacketHandler>();
                });

                connecter.ConnectHandler?.SocketDisconnectEvent.Register((_) =>
                {
                    container.Dispose();
                });

                connecter.ConnectHandler!.Packetizer.TryAdd(QueryBlockPacketFormatter.PacketTypeId,
                    new QueryBlockPacketFormatter());
                connecter.ConnectHandler!.Packetizer.TryAdd(LoginPacketFormatter.PacketTypeId,
                    new LoginPacketFormatter());
                connecter.ConnectHandler!.Packetizer.TryAdd(BlockInfoPacketFormatter.PacketTypeId,
                    new BlockInfoPacketFormatter());

                connecter.ConnectHandler!.Dispatcher.TryAdd(
                    BlockInfoPacketFormatter.PacketTypeId, container.Resolve<BlockInfoPacketHandler>());

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
