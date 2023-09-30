#region copyright
// This file(may named Utility.cs) is a part of the project: Utopia.Benchmark.
// 
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// 
// This file is part of Utopia.Benchmark.
//
// Utopia.Benchmark is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Utopia.Benchmark is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with Utopia.Benchmark. If not, see <https://www.gnu.org/licenses/>.
#endregion

namespace Utopia.Benchmark;
public static class Utility
{
    private readonly static Random _random = new();

    private static readonly SpinLock _lock = new();

    public static int RandomInt(int min = int.MinValue, int max = int.MaxValue)
    {
        bool entered = false;
        try
        {
            _lock.Enter(ref entered);

            return _random.Next(min, max);
        }
        finally
        {
            if (entered)
            {
                _lock.Exit();
            }
        }
    }

    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghizklmnopqrstuvwxyz";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[RandomInt(s.Length)]).ToArray());
    }

    public static string[] RandomStringArray(int length)
    {
        var result = new string[length];

        for (var index = 0; length != index; index++)
        {
            result[index] = RandomString(64);
        }

        return result;
    }

}
