// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using Autofac;
using NLog;
using Utopia.Core;
using Utopia.Core.Collections;
using Utopia.Core.Net.Packet;
using Utopia.Core.Plugin;
using Utopia.Core.Utilities;
using Utopia.Server.Entity;
using Utopia.Server.Map;
using Utopia.Server.Net;
using Utopia.Server.Plugin.Entity;

namespace Utopia.Server.Plugin;

public class Plugin : PluginForServer
{
    public required ISafeDictionary<Guuid, IWorldFactory> WorldFactories { private get; init; }

    public required ISafeDictionary<long,World> Worlds { private get; init; }

    public required IEntityManager EntityManager { private get; init; }

    public required IInternetMain InternetMain { private get; init; }

    public required ILifetimeScope Container { private get; init; }

    [ContainerBuild]
    public static void SetupContainer(ContainerBuilder builder)
    {
        builder.RegisterType<Generator>();
        builder.RegisterType<WorldFactory>();
        builder.RegisterType<GrassEntity>();
    }

    [LifecycleHandler(PluginLifeCycle.Activated)]
    public void ActivateEventHandler(ContainerBuilder builder)
    {
        WorldFactories.TryAdd(
                   IDs.WorldType,
                   Container.Resolve<WorldFactory>());

        var factory = new EmptyEntityFactory();
        factory.Entities.TryAdd(ResourcePack.Entity.GrassEntity.ID, Container.Resolve<GrassEntity>());

        EntityManager.TryAdd(ResourcePack.Entity.GrassEntity.ID,
            factory);

        // process query_map packet
        InternetMain.ClientCreatedEvent.Register(
                (e) =>
                {
                    Core.Net.IConnectHandler handler = e.Result!;

                    handler.Packetizer.EnterSync((list) =>
                    {
                        list.Add(new QueryBlockPacketFormatter());
                        list.Add(new LoginPacketFormatter());
                        list.Add(new BlockInfoPacketFormatter());
                    });

                    handler.Dispatcher.RegisterHandler(QueryBlockPacketFormatter.PacketTypeId,
                        (object packet) =>
                        {
                            var query = (QueryBlockPacket)packet;

                            Task.Run(() =>
                            {
                                if(!Worlds.TryGetValue(query.QueryPosition.Id,out World? world))
                                {
                                    return;
                                }

                                if (world!.TryGetBlock(query.QueryPosition.ToPos(), out IBlock? block))
                                {
                                    using var _ = block!.EnterWriteLock();

                                    var packet = new BlockInfoPacket();
                                    IReadOnlyCollection<IEntity> entities = block!.GetAllEntities();
                                    packet.Collidable = block.HasCollision;
                                    packet.Accessible = block.Accessible;
                                    packet.Position = query.QueryPosition;
                                    packet.Entities = entities.Select((i) => i.Id).ToArray();
                                    packet.EntityData = entities.Select((i) => i.ClientOnlyData()).ToArray();

                                    handler.WritePacket(BlockInfoPacketFormatter.PacketTypeId,
                                        packet
                                        );
                                }
                            });
                        });

                }
                );

    }

}
