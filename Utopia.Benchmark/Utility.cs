using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
