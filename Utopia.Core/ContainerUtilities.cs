// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace Utopia.Core;
public static class ContainerUtilities
{
    private class ContainerHolder
    {
        public IContainer Container { get; set; } = null!;
    }

    public static IContainer BuildWithIContainer(this ContainerBuilder builder)
    {
        var holder = new ContainerHolder();

        builder
            .Register(context =>
        {
            ContainerHolder container = holder;
            return container.Container;
        })
            .SingleInstance();

        var container = builder.Build();

        holder.Container = container;

        return container;
    }
}
