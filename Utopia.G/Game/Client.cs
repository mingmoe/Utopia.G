using Autofac;
using Castle.Core.Logging;
using Godot;
using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utopia.Core;
using Utopia.Core.Events;
using Utopia.Core.Translate;
using Utopia.Core.Utilities.IO;
using Utopia.G.Game.Entity;
using Utopia.Server;

namespace Utopia.G.Game;

public static class Client
{
    private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

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
            var tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

            foreach (var tcpi in tcpConnInfoArray)
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

        var port = option.Port;
        var locker = new object();

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
        // end

        var container = builder.Build();
        provider.TryRegisterService(container);

        // init filesystem
        provider.GetService<IFileSystem>().CreateIfNotExist();

        return provider;

        void register<T>(object instance) where T : notnull
        {
            provider.TryRegisterService<T>((T)instance);
            builder.RegisterInstance(instance).As<T>().ExternallyOwned();
        }
    }

    public static void Start(Uri server, Core.IServiceProvider service)
    {
        var bus = service.GetService<IEventBus>();

        try
        {
            stage(LifeCycle.InitializedSystem, () => { });

            stage(LifeCycle.LoadPlugin, () =>
            {
                service.GetService<IPluginLoader<IPlugin>>().Active(
                    service.GetService<IContainer>(),
                    typeof(Plugin.CorePlugin));
                service.GetService<IPluginLoader<IPlugin>>().LoadFromDirectory(
                    service.GetService<IFileSystem>().Plugins,
                    service.GetService<IContainer>(),
                    _logger
                    );
            });

            stage(LifeCycle.ConnectToServer, () =>
            {
                var connect = service.GetService<Net.ISocketConnecter>();

                var socket = connect.Connect(server);
            });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "the client initlize failed");
            stage(LifeCycle.Crash, () => { });
            stage(LifeCycle.Stop, () => { });
        }

        void stage(LifeCycle cycle, Action action)
        {
            bus!.Fire(new LifeCycleEvent(LifeCycleOrder.Before, cycle));
            action.Invoke();
            bus!.Fire(new LifeCycleEvent(LifeCycleOrder.After, cycle));
        }
    }

}
