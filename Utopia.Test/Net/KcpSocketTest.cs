// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utopia.Core.Net;

namespace Utopia.Test.Net;
public class KcpSocketTest
{
    [Fact]
    public async Task KcpTest()
    {
        var (client, server) = FakeSocket.Create();

        using var clientKcp = new KcpSocket(client);
        using var serverKcp = new KcpSocket(server);

        Assert.True(clientKcp.Alive);
        Assert.True(serverKcp.Alive);

        await clientKcp.Write((byte[])[1, 1, 4]);
        await clientKcp.Write((byte[])[5, 1, 4]);

        MemoryStream output = new();

        while (true)
        {
            byte[] buf = new byte[6];
            var read = await serverKcp.Read(buf);

            output.Write(buf, 0, read);

            if(output.Length >= 6)
            {
                break;
            }
        }

        Assert.Equal(0, await serverKcp.Read(new byte[1]));
        Assert.Equal(output.ToArray(), [1,1,4,5,1,4]);

        client.Shutdown();
        server.Shutdown();

        Assert.False(client.Alive);
        Assert.False(server.Alive);

        client.Dispose();
        server.Dispose();

        Assert.False(client.Alive);
        Assert.False(server.Alive);
    }
}
