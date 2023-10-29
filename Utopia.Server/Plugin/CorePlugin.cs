// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using Autofac;
using NLog;
using Utopia.Core;
using Utopia.Core.Collections;
using Utopia.Core.Events;
using Utopia.Core.Net.Packet;
using Utopia.Core.Plugin;
using Utopia.Core.Transition;
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

    private readonly object _lock = new();

    private readonly Logger _logger = LogManager.GetCurrentClassLogger();

    private readonly Core.IServiceProvider _serviceProvider;

    private readonly TranslateManager _translateManager;

    private readonly EventManager<LifeCycleEvent<PluginLifeCycle>> _lifecycleEvent = new();

    private PluginLifeCycle _lifeCycle = PluginLifeCycle.Unactivated;

    public PluginLifeCycle CurrentCycle
    {
        get
        {
            lock (_lock)
            {
                return _lifeCycle;
            }
        }
    }
    public IEventManager<LifeCycleEvent<PluginLifeCycle>> LifecycleEvent => _lifecycleEvent;

    private readonly ILifetimeScope _container;

    public CorePlugin(Core.IServiceProvider provider)
    {
        // set up servicd provider and other managers
        ArgumentNullException.ThrowIfNull(provider);
        _serviceProvider = provider;
        _translateManager = provider.GetService<TranslateManager>();
        IContainer container = provider.GetService<IContainer>();

        // build container
        System.Reflection.MethodInfo[] methods = GetType().GetMethods(
            System.Reflection.BindingFlags.Public
            | System.Reflection.BindingFlags.Static);

        ILifetimeScope scope = container.BeginLifetimeScope((builder) =>
        {
            foreach (System.Reflection.MethodInfo method in methods)
            {
                if (method.GetCustomAttributes(typeof(ContainerBuilderAttribute), true).Length != 0)
                {
                    _ = method.Invoke(this, new object[] { builder });
                }
            }
        });

        _container = scope;
    }

    [ContainerBuilder]
    public static void SetupContainer(ContainerBuilder builder)
    {
        _ = builder.RegisterType<Generator>();
        _ = builder.RegisterType<WorldFactory>();
        _ = builder.RegisterType<GrassEntity>();
    }

    [LifecycleHandler(PluginLifeCycle.Activated)]
    public void ActivateEventHandler()
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

    public ITranslatedString GetTranslation(TranslateKey key, object? data = null)
    {
        TranslateIdentifence? id = null;

        _serviceProvider.GetEventBusForService<TranslateIdentifence>().Register((e) =>
        {
            id = e.Target;
        });

        id = _serviceProvider.GetService<TranslateIdentifence>();

        return new ICUTranslatedString(key, _translateManager, id.Value, data ?? new object());
    }

    private void _SwitchLifecycle(PluginLifeCycle cycle)
    {
        lock (_lock)
        {
            _lifeCycle = cycle;
        }
    }

    private void _CallLifecycleHandlers(PluginLifeCycle cycle)
    {
        System.Reflection.MethodInfo[] methods = GetType().GetMethods(
                System.Reflection.BindingFlags.Public
                | System.Reflection.BindingFlags.Static);

        foreach (System.Reflection.MethodInfo method in methods)
        {
            object[] attributes = method.GetCustomAttributes(typeof(LifecycleHandlerAttribute), true);
            if (attributes.Length != 0)
            {
                if (!attributes.Any((attr) => ((LifecycleHandlerAttribute)attr).Lifecycle == cycle))
                {
                    return;
                }

                _ = method.Invoke(this, Array.Empty<object>());
            }
        }
    }

    public void Activate()
    {
        void @switch() => _SwitchLifecycle(PluginLifeCycle.Activated);

        void lifecycleCode() => _CallLifecycleHandlers(PluginLifeCycle.Activated);

        LifeCycleEvent<PluginLifeCycle>.EnterCycle(PluginLifeCycle.Activated, lifecycleCode, _logger, _lifecycleEvent, @switch);
    }

    public void Deactivate()
    {
        void @switch() => _SwitchLifecycle(PluginLifeCycle.Deactivated);

        void lifecycleCode() => _CallLifecycleHandlers(PluginLifeCycle.Deactivated);

        LifeCycleEvent<PluginLifeCycle>.EnterCycle(PluginLifeCycle.Deactivated, lifecycleCode, _logger, _lifecycleEvent, @switch);
    }
}
