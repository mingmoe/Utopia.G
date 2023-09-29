//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//

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
