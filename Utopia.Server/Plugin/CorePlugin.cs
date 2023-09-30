#region copyright
// This file(may named CorePlugin.cs) is a part of the project: Utopia.Server.
// 
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// 
// This file is part of Utopia.Server.
//
// Utopia.Server is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Utopia.Server is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with Utopia.Server. If not, see <https://www.gnu.org/licenses/>.
#endregion

using Utopia.Core.Collections;
using Utopia.Core.Utilities;
using Utopia.ResourcePack;
using Utopia.Server.Map;
using Utopia.Server.Plugin.Map;

namespace Utopia.Server.Plugin;

public class CorePlugin : CorePluginBase
{
    private Core.IServiceProvider _Provider { get; init; }

    public CorePlugin(Core.IServiceProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider);
        this._Provider = provider;
    }

    public override void Active()
    {
        this._Provider.GetService<SafeDictionary<Guuid, IWorldFactory>>().TryAdd(
                IDs.WorldType,
                new WorldFactory()
            );

    }
}
