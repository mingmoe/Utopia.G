#region copyright
// This file(may named GuuidTest.cs) is a part of the project: Utopia.Test.
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

using Utopia.Core.Utilities;

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
        var parsed = Guuid.CheckName(string.Empty);

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
