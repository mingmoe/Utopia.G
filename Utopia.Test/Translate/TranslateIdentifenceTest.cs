// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using Utopia.Core.Translation;

namespace Utopia.Test.Translate;

public class TranslateIdentifenceTest
{
    [Theory]
    [InlineData("123", "")]
    [InlineData("ASD", "321")]
    public void CheckTranslateIdConstruction(string first, string last) => Assert.Throws<ArgumentException>(() =>
                                                                                {
                                                                                    _ = new TranslateIdentifence(first, last);
                                                                                });

    [Fact]
    public void CheckTranslateIdToString()
    {
        var id = new TranslateIdentifence("zh", "cn");

        string str = id.ToString();

        Assert.Equal("zh_cn", str);
    }

    [Fact]
    public void TranslateIdParseTest()
    {
        var originId = new TranslateIdentifence("zh", "cn");
        string str = originId.ToString();

        var id = TranslateIdentifence.Parse(str);

        Assert.Equal(originId, id);
        Assert.Equal(originId.ToString(), id.ToString());
        Assert.Equal(originId.GetHashCode(), id.GetHashCode());
    }
}
