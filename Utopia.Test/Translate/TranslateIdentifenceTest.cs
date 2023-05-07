//===--------------------------------------------------------------===//
// Copyright (C) 2021-2023 mingmoe(me@kawayi.moe)(https://kawayi.moe)
// 
// This file is licensed under the MIT license.
// MIT LICENSE:https://opensource.org/licenses/MIT
//
//===--------------------------------------------------------------===//

namespace Utopia.Test.Translate;

public class TranslateIdentifenceTest
{
    [Theory]
    [InlineData("123", "")]
    [InlineData("ASD", "321")]
    public void CheckTranslateIdConstruction(string first, string last)
    {
        Assert.Throws<ArgumentException>(() =>
        {
            _ = new Core.Translate.TranslateIdentifence(first, last);
        });
    }

    [Fact]
    public void CheckTranslateIdToString()
    {
        var id = new Core.Translate.TranslateIdentifence("zho", "chn");

        var str = id.ToString();

        Assert.Equal("zho_chn", str);
    }
}
