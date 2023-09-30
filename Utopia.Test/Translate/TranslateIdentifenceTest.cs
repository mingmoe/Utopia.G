#region copyright
// This file(may named TranslateIdentifenceTest.cs) is a part of the project: Utopia.Test.
// 
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// 
// This file is part of Utopia.Test.
//
// Utopia.Test is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Utopia.Test is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with Utopia.Test. If not, see <https://www.gnu.org/licenses/>.
#endregion

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
