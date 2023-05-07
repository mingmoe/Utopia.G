//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//

namespace Utopia.Core;

public class ServiceChangedEvent<ServiceT> : Event<ServiceChangedType, ServiceT>, IServiceChangedEvent<ServiceT>
{
    public ServiceChangedEvent(ServiceChangedType param, ServiceT? initResult, bool cancelAble) : base(param, initResult, cancelAble)
    {
    }
}
