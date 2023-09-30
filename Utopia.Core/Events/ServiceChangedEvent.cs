#region copyright
// This file(may named ServiceChangedEvent.cs) is a part of the project: Utopia.Core.
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

namespace Utopia.Core.Events;

public class ServiceChangedEvent<ServiceT> : Event, IServiceChangedEvent<ServiceT>
{
    public ServiceChangedEvent(ServiceChangedType type, ServiceT param) : base(true)
    {
        this.Type = type;
        this.Target = param;
    }
    public ServiceT Target { get; set; }

    public ServiceT? Old { get; set; }

    public ServiceChangedType Type { get; init; }
}
