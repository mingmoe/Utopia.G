// This file was generated automatically

namespace Utopia.Tools.Generators;

public class GeneratorTemplate {

    public const string PluginClassTemplate =
        """
        ﻿using Autofac;
        using NLog;
        using Utopia.Core;
        using Utopia.Core.Events;
        using Utopia.Core.Plugin;
        using Utopia.Core.Transition;
        namespace Utopia.Server.Plugin;

        public abstract class PluginForServer : PluginInformation, IPlugin
        {

            private readonly object _lock = new();

            protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

            protected readonly Core.IServiceProvider _serviceProvider;

            protected readonly TranslateManager _translateManager;

            protected readonly EventManager<LifeCycleEvent<PluginLifeCycle>> _lifecycleEvent = new();

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

            protected readonly ILifetimeScope _container;

            public PluginForServer(Core.IServiceProvider provider)
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
        
        """;
}
