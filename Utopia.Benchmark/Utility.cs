using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utopia.Benchmark;
public static class Utility
{

    public static string RandomString(int length)
    {
        Random random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
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
