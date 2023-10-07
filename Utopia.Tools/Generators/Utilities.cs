namespace Utopia.Tools.Generators;

public static class Utilities
{

    public static bool NeedUpdateFile(string @out,params string[] @in)
    {
        var latestWriteTime = DateTime.MinValue;

        foreach(var file in @in)
        {
            var time = File.GetLastWriteTime(file);

            if(time > latestWriteTime)
            {
                latestWriteTime = time;
            }
        }

        var outputTime = File.GetLastWriteTime(@out);
        return latestWriteTime > outputTime;
    }

}
