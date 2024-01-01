// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Utopia.Core.Logging;

namespace Utopia.Test;
public static class ContainerManager
{
    public static readonly Lazy<IContainer> Container = new(() =>
    {
        LogManager.Init(new LogManager.LogOption()
        {
            ColorfulOutput = false,
            EnableConsoleDebugOutput = false,
            EnableConsoleOutput = false
        });

        ContainerBuilder builder = new();

        builder
            .RegisterType<NLogLoggerFactory>()
            .SingleInstance()
            .As<ILoggerFactory>();
        builder
            .RegisterGeneric(typeof(Logger<>))
            .As(typeof(ILogger<>))
            .SingleInstance();

        return builder.Build();
    }, true);
}
