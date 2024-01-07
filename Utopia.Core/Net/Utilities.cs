// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Spectre.Console;

namespace Utopia.Core.Net;
public static class Utilities
{
    public static async Task<PingReply?> TryPing(
        IPAddress address,
        int timeoutSeconds = 1,
        int retry = 3)
    {
        int tried = 0;

        while (tried < retry)
        {
            // try ping
            tried++;
            try
            {
                Ping pingTest = new();
                var reply = await pingTest.SendPingAsync(
                    address,
                    new TimeSpan(0, 0, timeoutSeconds));

                if (reply.Status != IPStatus.Success)
                {
                    continue;
                }

                return reply;
            }
            catch
            {
                continue;
            }
        }

        return null;
    }

}
