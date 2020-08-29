using System;

namespace Dom.Extensions
{
    public static class DateTimeExtensions
    {
        private static readonly DateTime ZeroData = new DateTime(2010, 10, 1, 0, 0, 20);

        public static DateTime StartOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
        }
        public static DateTime EndOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999);
        }
        public static bool IsToday(this DateTime date)
        {
            if (DateTime.Now.ToString("yy-MM-dd") == date.ToString("yy-MM-dd")) return true;
            return false;
        }
        public static int ToUnixTime(this DateTime date)
        {
            int unixTime = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            return unixTime;
        }
        public static bool IsYesterday(this DateTime date)
        {
            if (DateTime.Now.AddDays(-1).ToString("yy-MM-dd") == date.ToString("yy-MM-dd")) return true;
            return false;
        }
        public static byte[] DateTimeToBytes(this DateTime date)
        {
            var seconds = (date - ZeroData).TotalSeconds;
            return BitConverter.GetBytes(seconds);
        }
        public static int DateTimeToInt(this DateTime date)
        {
            return (int)(date - ZeroData).TotalSeconds;
        }

        public static Int64 GetJavascriptDateTime(this DateTime date)
        {
            Int64 retval = 0;
            var st = new DateTime(1970, 1, 1);
            TimeSpan t = (DateTime.Now.ToUniversalTime() - st);
            return retval = (Int64)(t.TotalMilliseconds + 0.5);
        }
    }
}
