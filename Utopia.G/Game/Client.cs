// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading;
using Autofac;
using Godot;
using Utopia.Core;
using Utopia.Core.Events;
using Utopia.Core.Plugin;
using Utopia.Core.Transition;
using Utopia.Core.Utilities.IO;
using Utopia.G.Game.Entity;
using Utopia.G.Graphy;
using Utopia.Server;

namespace Utopia.G.Game;

public static class Client
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    /// <summary>
    /// 创建本地服务器
    /// </summary>
    /// <returns>连接到本地服务器的地址</returns>
    public static Uri CreateLocalServer()
    {
        Launcher.LauncherOption option = new();

        // 查找可用端口
        bool portAvailable = true; // unkown
        do
        {
            var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

            foreach (TcpConnectionInformation tcpi in tcpConnInfoArray)
            {
                if (tcpi.LocalEndPoint.Port == option.Port)
                {
                    // try new port
                    portAvailable = false;
                    option.Port++;
                    break;
                }
            }

            if (option.Port > 25565)
            {
                throw new IOException("failed to find available socket(tcp) port!");
            }
        }
        while (!portAvailable);

        int port = option.Port;
        object locker = new();

        Thread thread = new(() =>
        {
            Launcher.Launch(option, locker);
        })
        {
            Name = "Server Thread"
        };
        thread.Start();

        Launcher.WaitForStart(locker);

        return new Uri("localhsot:" + port);
    }

    /// <summary>
    /// 初始化客户端.
    /// </summary>
    public static Core.IServiceProvider Initlize(Node root)
    {
        ServiceProvider provider = new();
        ContainerBuilder builder = new();

        // register
        register<IFileSystem>(new FileSystem());
        register<IEventBus>(new EventBus());
        register<IPluginLoader<IPlugin>>(new PluginLoader<IPlugin>());
        register<Net.ISocketConnecter>(new Net.SocketConnecter());
        register<ITranslateManager>(new TranslateManager());
        register<IEntityManager>(new EntityManager());
        register<Node>(root);
        register<TileManager>(new TileManager());
        // end
        _ = provider.TryRegisterService(builder);

        // init filesystem
        provider.GetService<IFileSystem>().CreateIfNotExist();

        return provider;

        void register<T>(object instance) where T : notnull
        {
            _ = provider.TryRegisterService<T>((T)instance);
            _ = builder.RegisterInstance(instance).As<T>().ExternallyOwned();
        }
    }

    public static void Start(Uri server, Core.IServiceProvider provider)
    {
        IEventBus bus = provider.GetService<IEventBus>();

        try
        {
            LifeCycleEvent<LifeCycle>.EnterCycle(LifeCycle.InitializedSystem, () => { },
                s_logger, bus, provider);

            LifeCycleEvent<LifeCycle>.EnterCycle(LifeCycle.LoadPlugin, () =>
            {
                provider.GetService<IPluginLoader<IPlugin>>().Register(
                    provider.GetService<ContainerBuilder>(),
                    typeof(Plugin.CorePlugin));
                provider.GetService<IPluginLoader<IPlugin>>().LoadFromDirectory(
                    provider.GetService<IFileSystem>().PluginsDirectory,
                    provider.GetService<ContainerBuilder>(),
                    s_logger
                    );
                IContainer container = provider.GetService<ContainerBuilder>().Build();
                provider.RemoveService<ContainerBuilder>();
                _ = provider.TryRegisterService(container);
                provider.GetService<IPluginLoader<IPlugin>>().Active(container);
            },
                s_logger, bus, provider);

            LifeCycleEvent<LifeCycle>.EnterCycle(LifeCycle.ConnectToServer, () =>
            {
                Net.ISocketConnecter connect = provider.GetService<Net.ISocketConnecter>();

                Core.Net.IConnectHandler socket = connect.Connect(server);
            },
                s_logger, bus, provider);
        }
        catch (Exception ex)
        {
            s_logger.Error(ex, "the client initlize failed");
            LifeCycleEvent<LifeCycle>.EnterCycle(LifeCycle.Crash, () => { },
                s_logger, bus, provider);
            LifeCycleEvent<LifeCycle>.EnterCycle(LifeCycle.Stop, () => { },
                s_logger, bus, provider);
        }
    }
}
