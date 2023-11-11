// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

namespace Utopia.Tools.Generators;

public static class Utilities
{

    public static bool NeedUpdateFile(string @out, params string[] @in)
    {
        DateTime latestWriteTime = DateTime.MinValue;

        foreach (string file in @in)
        {
            DateTime time = File.GetLastWriteTime(file);

            if (time > latestWriteTime)
            {
                latestWriteTime = time;
            }
        }

        DateTime outputTime = File.GetLastWriteTime(@out);
        return latestWriteTime > outputTime;
    }
}
