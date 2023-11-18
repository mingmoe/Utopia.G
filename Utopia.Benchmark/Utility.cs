// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Text;

namespace Utopia.Benchmark;
public static class Utility
{
    private static readonly Random s_random = new();

    private static readonly object s_lock = new();

    public static int RandomInt(int min = int.MinValue, int max = int.MaxValue)
    {
        lock (s_lock)
        {
            return s_random.Next(min, max);
        }

    }

    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghizklmnopqrstuvwxyz";

        StringBuilder @string = new();

        for(var s = 0; s != length; s++)
        {
            @string.Append(chars[RandomInt(0, chars.Length - 1)]);
        }

        return @string.ToString();
    }

    public static string[] RandomStringArray(int length)
    {
        string[] result = new string[length];

        for (int index = 0; length != index; index++)
        {
            result[index] = RandomString(64);
        }

        return result;
    }

}
