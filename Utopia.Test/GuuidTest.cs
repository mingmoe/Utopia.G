//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//
using Utopia.Core;

namespace Utopia.Test;

public class GuuidTest
{

    [Fact]
    public void TestGuuidToStringAndParseStringWorksWell()
    {
        var guuid = new Guuid("root", "node");
        var str = guuid.ToString();

        var parsed = Guuid.ParseString(str);

        Assert.Equal(guuid, parsed);
    }

    [Fact]

    public void CheckIllegalNames()
    {
        var parsed = Guuid.CheckNameIllegal(string.Empty);

        Assert.False(parsed);
    }

    [Theory]
    [InlineData("", "non-empty")]
    [InlineData("non-empty", "")]
    public void TestGuuidParseStringParseIllegal(string root, string node)
    {
        Assert.Throws<ArgumentException>(() =>
        {
            _ = new Guuid(root, node);
        });
    }
}
