// This file was generated automatically

namespace Utopia.Tools.Generators;

public class GeneratorTemplate {

    public const string ServerPluginClassTemplate =
        """
        using Autofac;
        using Utopia.Core;
        using Utopia.Core.Events;
        using Utopia.Core.Plugin;
        using System.CodeDom.Compiler;
        $PluginInformationNamespace$
        namespace Utopia.Server.Plugin;

        [GeneratedCode("$GENERATOR_NAME$","$GENERATOR_VERSION$")]
        public abstract class PluginForServer : PluginInformation, IPlugin
        {
            private readonly object _lock = new();

            public required ILogger Logger { get; init; }

            public required TranslateManager TranslateManager { get; init; }

            protected readonly EventManager<LifeCycleEvent<PluginLifeCycle>> _lifecycleEvent = new();

            private PluginLifeCycle _lifeCycle = PluginLifeCycle.Created;

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

            public PluginForServer(IContainer container)
            {
                // set up service provider and other managers
                ArgumentNullException.ThrowIfNull(container);

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

                LifeCycleEvent<PluginLifeCycle>.EnterCycle(PluginLifeCycle.Activated, lifecycleCode, Logger, _lifecycleEvent, @switch);
            }

            public void Deactivate()
            {
                void @switch() => _SwitchLifecycle(PluginLifeCycle.Deactivated);

                void lifecycleCode() => _CallLifecycleHandlers(PluginLifeCycle.Deactivated);

                LifeCycleEvent<PluginLifeCycle>.EnterCycle(PluginLifeCycle.Deactivated, lifecycleCode, Logger, _lifecycleEvent, @switch);
            }
        }
        
        """;
}
