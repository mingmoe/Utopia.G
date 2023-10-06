#region copyright
// This file(may named CorePlugin.cs) is a part of the project: Utopia.Server.
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

using Autofac;
using Utopia.Core.Collections;
using Utopia.Core.Net.Packet;
using Utopia.Core.Utilities;
using Utopia.ResourcePack;
using Utopia.Server.Entity;
using Utopia.Server.Map;
using Utopia.Server.Net;
using Utopia.Server.Plugin.Entity;
using Utopia.Server.Plugin.Map;
using Utopia.Server.Plugin.Net;

namespace Utopia.Server.Plugin;

public class CorePlugin : PluginInformation, IPlugin
{
    private Core.IServiceProvider _Provider { get; init; }

    private ILifetimeScope _container;

    public CorePlugin(Core.IServiceProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider);
        this._Provider = provider;
        var container = provider.GetService<IContainer>();

        var scope = container.BeginLifetimeScope((builder) =>
        {
            builder.RegisterType<Generator>();
            builder.RegisterType<WorldFactory>();
            builder.RegisterType<GrassEntity>();
        });
        this._container = scope;
    }

    public void Active()
    {
        this._Provider.GetService<SafeDictionary<Guuid, IWorldFactory>>().TryAdd(
                IDs.WorldType,
                this._container.Resolve<WorldFactory>());

        this._Provider.TryRegisterService<IInternetListener>(
            new InternetListener());

        var factory = new EmptyEntityFactory();
        factory.Entities.TryAdd(ResourcePack.Entity.GrassEntity.ID, this._container.Resolve<GrassEntity>());

        this._Provider.GetService<IEntityManager>().TryAdd(ResourcePack.Entity.GrassEntity.ID,
            factory);

        // process query_map packet
        this._Provider.GetService<InternetMain>().ClientCreatedEvent.Register(
                (e) =>
                {
                    var handler = e.Result!;

                    handler.Packetizer.OperateFormatterList((list) =>
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
                                if (this._Provider.TryGetBlock(query.QueryPosition, out var block))
                                {
                                    var packet = new BlockInfoPacket();
                                    var entities = block!.GetAllEntities();
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

    public void Dispose()
    {
        this._container.Dispose();
        GC.SuppressFinalize(this);
    }
}
