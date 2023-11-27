// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading;
using Autofac;
using Godot;
using Microsoft.Extensions.Logging;
using Utopia.Core;
using Utopia.Core.Events;
using Utopia.Core.Plugin;
using Utopia.Core.Translation;
using Utopia.Core.Utilities.IO;
using Utopia.G.Game.Entity;
using Utopia.G.Graphy;
using Utopia.G.Net;
using Utopia.Server;

namespace Utopia.G.Game;

public class Client
{
    public required ILogger Logger { get; init; }

    public required IEventBus EventBus { get; init; }

    public required IPluginLoader<IPlugin> PluginLoader { get; init; }

    public required IFileSystem FileSystem { get; init; }

    public required ISocketConnecter SocketConnecter { get;init; }

    public required PluginSearcher<IPlugin> PluginSearcher { get; init; }

    /// <summary>
    /// 创建本地服务器
    /// </summary>
    /// <returns>连接到本地服务器的地址</returns>
    public Uri CreateLocalServer()
    {
        Launcher.LauncherOption option = new();

        // 查找可用端口
        bool portAvailable = true; // unknown
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

        // start the server
        int port = option.Port;
        CancellationTokenSource source = new();

        Thread thread = new(() =>
        {
            Launcher.Launch(option, source);
        })
        {
            Name = "Server Thread"
        };
        thread.Start();

        SpinWait wait = new();
        while (!source.IsCancellationRequested)
        {
            wait.SpinOnce();
        }

        return new Uri("localhsot:" + port);
    }

    /// <summary>
    /// 初始化客户端.
    /// </summary>
    public static IContainer Initialize(Node root)
    {
        ContainerBuilder builder = new();

        // register
        builder
            .RegisterType<FileSystem>()
            .SingleInstance()
            .As<IFileSystem>();
        builder
            .RegisterType<EventBus>()
            .SingleInstance()
            .As<IEventBus>();
        builder
            .RegisterType<PluginLoader<IPlugin>>()
            .SingleInstance()
            .As<IPluginLoader<IPlugin>>();
        builder
            .RegisterType<SocketConnecter>()
            .SingleInstance()
            .As<ISocketConnecter>();
        builder
            .RegisterType<TranslateManager>()
            .SingleInstance()
            .As<ITranslateManager>();
        builder
            .RegisterType<EntityManager>()
            .SingleInstance()
            .As<IEntityManager>();
        builder
            .RegisterInstance(root);
        builder
            .RegisterType<TileManager>()
            .SingleInstance()
            .AsSelf();
        builder.RegisterType<PluginSearcher<IPlugin>>()
            .AsSelf()
            .SingleInstance();
        builder.RegisterType<Client>()
            .SingleInstance()
            .AsSelf();
        builder.RegisterType<Plugin.CorePlugin>()
            .SingleInstance()
            .AsSelf();

        return builder.Build();
    }

    public void Start(Uri server, IContainer container)
    {
        try
        {
            LifeCycleEvent<LifeCycle>.EnterCycle(LifeCycle.InitializedSystem, () => { },
                Logger, EventBus, () => { });

            LifeCycleEvent<LifeCycle>.EnterCycle(LifeCycle.LoadPlugin, () =>
            {
                PluginLoader.AddPlugin(PluginSearcher.CreatePluginContext(
                    container.Resolve<Plugin.CorePlugin>(),
                    new PackedPluginManifest(),
                    container.BeginLifetimeScope(),
                    FileSystem.RootDirectory,
                    null,
                    string.IsNullOrWhiteSpace(Assembly.GetExecutingAssembly().Location)
                        ? null
                        : Assembly.GetExecutingAssembly().Location,
                    FileSystem.GetConfigurationDirectoryOfPlugin(container.Resolve<Plugin.CorePlugin>())
                    ));

                foreach(var plugin in
                    PluginSearcher.LoadAllPackedPluginsFromDirectory(FileSystem.PackedPluginsDirectory))
                {
                    PluginLoader.AddPlugin(plugin);
                }
                PluginLoader.ActiveAllPlugins();
            },
                Logger, EventBus, () => { });

            LifeCycleEvent<LifeCycle>.EnterCycle(LifeCycle.ConnectToServer, () =>
            {
                Core.Net.IConnectHandler socket = SocketConnecter.Connect(server);

                // TODO
            },
                Logger, EventBus, () => { });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "the client initialize failed");
            LifeCycleEvent<LifeCycle>.EnterCycle(LifeCycle.Crash, () => { },
                Logger, EventBus, () => { });
        }
        finally
        {
            LifeCycleEvent<LifeCycle>.EnterCycle(LifeCycle.Stop, () => { },
                Logger, EventBus, () => { });
        }
    }
}
