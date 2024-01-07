// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Microsoft.Extensions.Logging;
using Moq;
using Utopia.Core.Net;
using Utopia.Core.Utilities;

namespace Utopia.Test.Net;
public class ConnecterTest
{
    [Fact]
    public async void ConnectHandlerTest()
    {
        // prepare
        var (client, server) = FakeSocket.Create();

        using var clientHandler = new ConnectHandler(client)
        {
            Logger = ContainerManager.Container.Value.Resolve<ILogger<ConnectHandler>>(),
            Container = null!
        };
        using var serverHandler = new ConnectHandler(server)
        {
            Logger = ContainerManager.Container.Value.Resolve<ILogger<ConnectHandler>>(),
            Container = null!
        };

        var clientTask = Task.Run(clientHandler.InputLoop);
        var serverTask = Task.Run(serverHandler.InputLoop);
        var all = Task.WhenAll(clientTask, serverTask);

        var packet = new Guuid("root", "packet");
        var data = (byte[])([1, 1, 4, 5, 1, 4]);

        Mock<IPacketFormatter> formatter = new();

        formatter.Setup((f) => f.GetValue(It.IsAny<Guuid>(),It.IsAny<ReadOnlySequence<byte>>()))
            .Returns((Guuid _,ReadOnlySequence<byte> a) =>
        {
            return a.ToArray();
        });
        formatter.Setup((f) => f.ToPacket(It.IsAny<Guuid>(),It.IsAny<object>()))
            .Returns((Guuid id, object a) =>
            {
                Assert.Equal(packet, id);
                return (Memory<byte>)(byte[])a;
            });

        serverHandler.Packetizer.TryAdd(packet, formatter.Object);
        clientHandler.Packetizer.TryAdd(packet, formatter.Object);
        bool received = false;

        Mock<IPacketHandler> handler = new();
        handler.Setup((h) => h.Handle(It.IsAny<Guuid>(), It.IsAny<object>()))
            .Returns((Guuid id,object rev) =>
            {
                Assert.Equal(packet, id);
                if (Enumerable.SequenceEqual((byte[])rev, data))
                {
                    received = true;
                }
                return Task.CompletedTask;
            });

        serverHandler.Dispatcher.TryAdd(packet, handler.Object);

        // write
        Thread.Sleep(500);

        Assert.True(serverHandler.Running);
        Assert.True(clientHandler.Running);

        clientHandler.WritePacket(packet, data);

        Thread.Sleep(500);

        clientHandler.Disconnect();
        serverHandler.Disconnect();

        await all;

        // check
        Assert.True(received);

        Assert.False(serverHandler.Running);
        Assert.False(clientHandler.Running);
    }
}
