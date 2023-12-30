// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Utopia.Core;
public static class TimeUtilities
{
    public static void SetAnNoticeWhenCancel(ILogger logger,string name,CancellationTokenSource source)
    {
        Stopwatch stopwatch = new();
        stopwatch.Start();
        Task.Run(() =>
        {
            SpinWait wait = new();
            while (!source.IsCancellationRequested)
            {
                wait.SpinOnce();
            }
            stopwatch.Stop();
            logger.LogInformation(
                $"{name} started,using {{}} s {{}} ms", stopwatch.Elapsed.Seconds, stopwatch.Elapsed.Milliseconds);
        });
    }

}
