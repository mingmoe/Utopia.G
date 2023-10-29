// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using Utopia.Core.Utilities;

namespace Utopia.Test;

public class GuuidTest
{

    [Fact]
    public void TestGuuidToStringAndParseStringWorksWell()
    {
        var guuid = new Guuid("root", "node");
        string str = guuid.ToString();

        var parsed = Guuid.Parse(str);

        Assert.Equal(guuid, parsed);
        Assert.Equal(guuid.GetHashCode(), parsed.GetHashCode());
    }

    [Fact]

    public void CheckIllegalNames()
    {
        bool parsed = Guuid.CheckName(string.Empty);

        Assert.False(parsed);
    }

    [Theory]
    [InlineData("", "non-empty")]
    [InlineData("non-empty", "")]
    public void TestGuuidParseStringParseIllegal(string root, string node) => Assert.Throws<ArgumentException>(() =>
                                                                                   {
                                                                                       _ = new Guuid(root, node);
                                                                                   });
}
