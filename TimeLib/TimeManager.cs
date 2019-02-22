using System;

namespace TimeLib
{
    public class TimeManager
    {
        public static long GetUtcDatetimeInMilliseconds()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
    }
}
