// This file was generated automatically

namespace Utopia.Tools.Generators;

public class GeneratorTemplate {

    public const string ServerPluginClassTemplate =
        """
        using Utopia.Core;
        using Utopia.Core.Events;
        using Utopia.Core.Plugin;
        using System.CodeDom.Compiler;
        using Microsoft.Extensions.Logging;
        using Utopia.Core.Translation;
        $PluginInformationNamespace$
        namespace Utopia.Server.Plugin;

        [GeneratedCode("$GENERATOR_NAME$","$GENERATOR_VERSION$")]
        public abstract class PluginForServer : PluginInformation, IPlugin
        {
            private readonly object _lock = new();
                
            public required ILogger<PluginForServer> Logger { private get; init; }
                
            public required ITranslationGetter I { protected get; init; }

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
            
            public PluginForServer()
            {
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
                Created();

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

                
            /// <summary>
            /// Fire <see cref="PluginLifeCycle.Created"/> events and call handler methods(with <see cref="LifecycleHandlerAttribute"/>).
            /// </summary>
            public void Created()
            {
                void @switch() => _SwitchLifecycle(PluginLifeCycle.Created);

                void lifecycleCode() => _CallLifecycleHandlers(PluginLifeCycle.Created);

                LifeCycleEvent<PluginLifeCycle>.EnterCycle(PluginLifeCycle.Created, lifecycleCode, Logger, _lifecycleEvent, @switch);
            }
        }
        
        """;
}
