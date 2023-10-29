// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

namespace Utopia.Benchmark;
public static class Utility
{
    private static readonly Random s_random = new();

    private static readonly SpinLock s_lock = new();

    public static int RandomInt(int min = int.MinValue, int max = int.MaxValue)
    {
        bool entered = false;
        try
        {
            s_lock.Enter(ref entered);

            return s_random.Next(min, max);
        }
        finally
        {
            if (entered)
            {
                s_lock.Exit();
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
        string[] result = new string[length];

        for (int index = 0; length != index; index++)
        {
            result[index] = RandomString(64);
        }

        return result;
    }

}
