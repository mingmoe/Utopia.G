// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Utopia.Core.Collections;
using Utopia.Core;
using Utopia.Core.Events;
using Utopia.Server.Logic;
using Utopia.Server.Map;
using Utopia.Server.Net;
using Microsoft.Extensions.DependencyInjection;
using static Utopia.Server.Launcher;
using Utopia.Core.Plugin;
using Utopia.Core.Utilities;
using System.ComponentModel;
using Autofac;
using SharpCompress;
using System.Reflection;
using Utopia.Core.IO;

namespace Utopia.Server;

public sealed class Headquarters
{
    private readonly CancellationTokenSource _logicThreadStartWaitTokenSource = new();

    private readonly CancellationTokenSource _internetThreadStartWaitTokenSource = new();

    public required ILogger<Headquarters> Logger { get; init; }

    public required IEventBus EventBus { get; init; }

    public required IPluginLoader<IPlugin> PluginLoader { get; init; }

    public required IFileSystem FileSystem { get; init; }

    public required Autofac.IContainer Container { get; init; }

    public required ILauncherOption Option { get; init; }

    public required ILogicThread LogicThread { get; init; }

    public required IInternetMain InternetMain { get; init; }

    public required IInternetListener InternetListener { get;init; }

    public required ISafeDictionary<long, IWorld> Worlds { get; init; }

    public required ISafeDictionary<Guuid, IWorldFactory> WorldFactories { get; init; }

    public required PluginHelper<IPlugin> PluginSearcher { get; init; }

    public LifeCycle CurrentLifeCycle { get; private set; }

    private void _StartLogicThread()
    {
        var logicT = new Thread(() =>
        {
            KeyValuePair<long, IWorld>[] worlds = Worlds.ToArray();
            foreach (KeyValuePair<long, IWorld> world in worlds)
            {
                LogicThread.Add(world.Value);
            }
            // 注册关闭事件
            EventBus.Register<LifeCycleEvent<LifeCycle>>((cycle) =>
            {
                if (cycle.Cycle == LifeCycle.Stop && cycle.Order == LifeCycleOrder.After)
                {
                    LogicThread.StopTokenSource.Cancel();
                }
            });
            try
            {
                ((StandardLogicThread)LogicThread).Run(_logicThreadStartWaitTokenSource);
            }
            catch(Exception ex)
            {
                Logger.LogError(ex, "Logic Thread Crashed");
            }
            finally {
                LogicThread.StopTokenSource.Cancel();
            }
        })
        {
            Name = "Server Logic Thread"
        };
        logicT.Start();
    }


    /// <summary>
    /// 启动网络线程
    /// </summary>
    private void _StartNetworkThread()
    {
        InternetListener.Listen(Option.Port);
        Logger.LogInformation("listen to {port}", Option.Port);

        InternetMain netThread = (InternetMain)InternetMain;
        var thread = new Thread(() =>
        {
            // 注册关闭事件
            EventBus.Register<LifeCycleEvent<LifeCycle>>((cycle) =>
            {
                if (cycle.Cycle == LifeCycle.Stop && cycle.Order == LifeCycleOrder.After)
                {
                    netThread.StopTokenSource.Cancel();
                }
            });
            try
            {
                netThread.Run(_internetThreadStartWaitTokenSource);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Internet Thread Crashed");
            }
            finally
            {
                netThread.StopTokenSource.Cancel();
            }
        })
        {
            Name = "Server Networking Thread"
        };
        thread.Start();
    }

    public void _LoadSave()
    {
        KeyValuePair<Guuid, IWorldFactory>[] array = WorldFactories.ToArray();

        // TODO: Load saves from file
        if (array.Length == 1)
        {
            Worlds.TryAdd(0, array[0].Value.GenerateNewWorld());
        }
    }

    /// <summary>
    /// 使用参数启动服务器
    /// </summary>
    /// <param name="startTokenSource">when the server started,cancel the token</param>
    public void Launch(CancellationTokenSource startTokenSource)
    {
        ArgumentNullException.ThrowIfNull(startTokenSource);

        Thread.CurrentThread.Name = "Server Headquarters Thread";

        void changeLifecycle(LifeCycle lifeCycle, Action action)
        {

            LifeCycleEvent<LifeCycle>.EnterCycle(
                lifeCycle,
                action,
                Logger,
                EventBus, () =>
                {
                    CurrentLifeCycle = lifeCycle;
                });
        }

        try
        {
            changeLifecycle(LifeCycle.InitializedSystem, () => { return; });

            // 加载插件
            changeLifecycle(LifeCycle.LoadPlugin, () =>
            {
                PluginLoader.AddPlugin(
                    PluginSearcher.BuildPluginFromType(
                        typeof(Plugin.Plugin),
                        FileSystem.RootDirectory,
                        null,
                        string.IsNullOrWhiteSpace(Assembly.GetExecutingAssembly().Location)
                            ? null
                            : Assembly.GetExecutingAssembly().Location,
                        new()));

                foreach(var plugin in
                    PluginSearcher.LoadAllPackedPluginsFromDirectory(
                        FileSystem.PackedPluginsDirectory))
                {
                    PluginLoader.AddPlugin(plugin);
                }
                PluginLoader.ActiveAllPlugins();
            });

            // 创建世界
            changeLifecycle(LifeCycle.LoadSavings, _LoadSave);

            // 设置逻辑线程
            changeLifecycle(LifeCycle.StartLogicThread, _StartLogicThread);

            // 设置网络线程
            changeLifecycle(LifeCycle.StartNetThread, _StartNetworkThread);

            var wait = new SpinWait();

            var netToken = InternetMain.StopTokenSource;
            var logicToken = LogicThread.StopTokenSource;

            using var stopTokenSource = CancellationTokenSource.CreateLinkedTokenSource(netToken.Token, logicToken.Token);

            // wait for starting
            while (!(_internetThreadStartWaitTokenSource.IsCancellationRequested
                && _logicThreadStartWaitTokenSource.IsCancellationRequested))
            {
                wait.SpinOnce();

                // 出师未捷身先死
                if (stopTokenSource.IsCancellationRequested)
                {
                    return;
                }
            };
            startTokenSource.CancelAfter(100/* wait for fun :-) */);

            // stop when any of threads stop
            while (!stopTokenSource.IsCancellationRequested)
            {
                wait.SpinOnce();
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "the server crash");
            changeLifecycle(LifeCycle.Crash, () => { });
        }
        finally
        {
            Logger.LogInformation("stop");
            changeLifecycle(LifeCycle.Stop, () => { });
        }
    }
}
