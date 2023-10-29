// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using Autofac;
using NLog;
using Utopia.Core.Collections;
using Utopia.Core.Net.Packet;
using Utopia.Core.Plugin;
using Utopia.Core.Utilities;
using Utopia.Server.Entity;
using Utopia.Server.Map;
using Utopia.Server.Net;
using Utopia.Server.Plugin.Entity;
using Utopia.Server.Plugin.Map;
using Utopia.Server.Plugin.Net;

namespace Utopia.Server.Plugin;

public class Plugin : PluginForServer
{
    public Plugin(Core.IServiceProvider provider) : base(provider)
    {
    }

    [ContainerBuilder]
    public static void SetupContainer(ContainerBuilder builder)
    {
        builder.RegisterType<Generator>();
        builder.RegisterType<WorldFactory>();
        builder.RegisterType<GrassEntity>();
    }

    [LifecycleHandler(PluginLifeCycle.Activated)]
    public void ActivateEventHandler(ContainerBuilder builder)
    {
        _ = _serviceProvider.GetService<SafeDictionary<Guuid, IWorldFactory>>().TryAdd(
                   IDs.WorldType,
                   _container.Resolve<WorldFactory>());

        _ = _serviceProvider.TryRegisterService<IInternetListener>(
            new InternetListener());

        var factory = new EmptyEntityFactory();
        _ = factory.Entities.TryAdd(ResourcePack.Entity.GrassEntity.ID, _container.Resolve<GrassEntity>());

        _ = _serviceProvider.GetService<IEntityManager>().TryAdd(ResourcePack.Entity.GrassEntity.ID,
            factory);

        // process query_map packet
        _serviceProvider.GetService<InternetMain>().ClientCreatedEvent.Register(
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

                            _ = Task.Run(() =>
                            {
                                if (_serviceProvider.TryGetBlock(query.QueryPosition, out IBlock? block))
                                {
                                    var packet = new BlockInfoPacket();
                                    IReadOnlyCollection<IEntity> entities = block!.GetAllEntities();
                                    packet.Collidable = block.Collidable;
                                    packet.Accessible = block.Accessable;
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
