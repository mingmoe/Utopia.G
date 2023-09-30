#region copyright
// This file(may named Client.cs) is a part of the project: Utopia.G.
// 
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// 
// This file is part of Utopia.G.
//
// Utopia.G is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Utopia.G is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with Utopia.G. If not, see <https://www.gnu.org/licenses/>.
#endregion

using Autofac;
using Godot;
using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading;
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
