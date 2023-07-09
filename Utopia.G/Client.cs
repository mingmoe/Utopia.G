using Autofac;
using Castle.Core.Logging;
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
using Utopia.Core.Translate;
using Utopia.Server;

namespace Utopia.G;

public static class Client
{
    private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

    /// <summary>
    /// 创建本地服务器
    /// </summary>
    /// <returns>连接到本地服务器的地址</returns>
    public static Uri CreateLocalServer()
    {
        Utopia.Server.Launcher.LauncherOption option = new();

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

        Thread thread = new(() =>
        {
            Launcher.Launch(option);
        })
        {
            Name = "Server Thread"
        };
        thread.Start();

        return new Uri("localhsot:" + port);
    }

    private static Core.IServiceProvider _Initlize()
    {
        ServiceProvider provider = new();
        ContainerBuilder builder = new();

        // register
        register<IFileSystem>(new FileSystem());
        register<IEventBus>(new EventBus());
        register<IPluginLoader<IPlugin>>(new EventBus());
        register<Net.ISocketConnecter>(new Net.SocketConnecter());
        // end

        var container = builder.Build();
        provider.TryRegisterService<IContainer>(container);

        // init filesystem
        provider.GetService<IFileSystem>().CreateIfNotExist();

        return provider;

        void register<T>(object instance) where T : notnull
        {
            provider.TryRegisterService<T>((T)instance);
            builder.RegisterInstance(instance).As<T>().ExternallyOwned();
        }
    }

    private static void _LoadPlugin(Utopia.Core.IServiceProvider provider)
    {
        var loader = provider.GetService<IPluginLoader<IPlugin>>();
        var fs = provider.GetService<IFileSystem>();
        foreach (var f in Directory.GetFiles(fs.Plugins, "*.dll", System.IO.SearchOption.AllDirectories))
        {
            var file = Path.GetFullPath(f);
            _logger.Info("loading plugin from dll:{plugin}", file);
            loader.Active(provider.GetService<IContainer>(), file);
        }
    }

    public static void Start(Uri server)
    {
        var service = _Initlize();

        var bus = service.GetService<IEventBus>();

        try
        {
            stage(LifeCycle.InitializedSystem, () => { });

            stage(LifeCycle.LoadPlugin, () => { _LoadPlugin(service); });

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
