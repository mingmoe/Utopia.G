#region copyright
// This file(may named PluginLoader.cs) is a part of the project: Utopia.Core.
// 
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// 
// This file is part of Utopia.Core.
//
// Utopia.Core is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Utopia.Core is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with Utopia.Core. If not, see <https://www.gnu.org/licenses/>.
#endregion

using Autofac;
using NLog;
using System.Collections.Immutable;
using Utopia.Core;
using Utopia.Core.Events;

namespace Utopia.Server;

/// <summary>
/// 插件加载器
/// </summary>
public class PluginLoader<PluginT> : IPluginLoader<PluginT> where PluginT : IPluginBase
{
    readonly object _locker = new();

    List<(Type, PluginT)> _ActivePlugins { get; } = new();

    List<Type> _UnresolvedPlugins { get; } = new();

    readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public ImmutableArray<(Type, PluginT)> ActivedPlugins
    {
        get
        {
            lock (this._locker)
            {
                return this._ActivePlugins.ToImmutableArray();
            }
        }
    }

    public ImmutableArray<Type> UnregisterPlugins
    {
        get
        {
            lock (this._locker)
            {
                return this._UnresolvedPlugins.ToImmutableArray();
            }
        }
    }

    public void AddUnresolved(Type type)
    {
        ArgumentNullException.ThrowIfNull(type, nameof(type));
        lock (this._locker)
        {
            this._UnresolvedPlugins.Add(type);
        }
    }

    public IEventManager<IPluginLoader<PluginT>.ActivePlguinEventArgs> ActivePlguinEvent { get; }
    = new EventManager<IPluginLoader<PluginT>.ActivePlguinEventArgs>();

    public ImmutableArray<Type> UnresolveredPlugins => throw new NotImplementedException();

    public void Active(IContainer container)
    {
        ArgumentNullException.ThrowIfNull(container, nameof(container));

        lock (this._locker)
        {
            foreach (var type in this._UnresolvedPlugins)
            {
                if (!type.IsAssignableTo(typeof(PluginT)))
                {
                    throw new ArgumentException($"try to active a type without implening {typeof(PluginT)} interafce");
                }
                var p = (PluginT)container.Resolve(type);
                var args = new IPluginLoader<PluginT>.ActivePlguinEventArgs(type, container)
                {
                    PluginInstance = p
                };
                this.ActivePlguinEvent.Fire(args);

                if (args.PluginInstance == null)
                {
                    return;
                }

                p = args.PluginInstance;

                try
                {
                    p.Active();
                }
                catch (Exception e)
                {
                    this._logger.Error(e,
                        "failed to active plugin:`{pluginName}`(ID {pluginId}) try to access {pluginHomepage} for help",
                        p.Name,
                        p.Id,
                        p.Homepage);
                }

                lock (this._locker)
                {
                    this._ActivePlugins.Add((type, p));
                }
            }
            this._UnresolvedPlugins.Clear();
        }
    }
}
