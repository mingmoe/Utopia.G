// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

namespace Utopia.Core.Events;

public class ServiceChangedEvent<ServiceT> : Event, IServiceChangedEvent<ServiceT>
{
    public ServiceChangedEvent(ServiceChangedType type, ServiceT param)
    {
        Type = type;
        Target = param;
    }
    public ServiceT Target { get; set; }

    public ServiceT? Old { get; set; }

    public ServiceChangedType Type { get; init; }
}
