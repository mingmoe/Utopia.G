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
        var (client, server) = FakeSocket.Create();

        using var clientHandler = new ConnectHandler(client)
        { logger = ContainerManager.Container.Value.Resolve<ILogger<ConnectHandler>>() };
        using var serverHandler = new ConnectHandler(server)
        { logger = ContainerManager.Container.Value.Resolve<ILogger<ConnectHandler>>() };

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
            .Returns((Guuid _, object a) =>
            {
                return (Memory<byte>)(byte[])a;
            });

        serverHandler.Packetizer.TryAdd(packet, formatter.Object);
        clientHandler.Packetizer.TryAdd(packet, formatter.Object);

        bool check = false;

        serverHandler.Dispatcher.RegisterHandler(packet, (rev) =>
        {
            if (Enumerable.SequenceEqual((byte[])rev,data))
            {
                check = true;
            }
        });

        Thread.Sleep(500);

        Assert.True(serverHandler.Running);
        Assert.True(clientHandler.Running);

        clientHandler.WritePacket(packet, data);

        Thread.Sleep(500);

        clientHandler.Disconnect();
        serverHandler.Disconnect();

        await all;

        Assert.True(check);

        Assert.False(serverHandler.Running);
        Assert.False(clientHandler.Running);
    }
}
