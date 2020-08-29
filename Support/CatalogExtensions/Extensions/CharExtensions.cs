using System;

namespace Dom.Extensions
{
    public static class CharExtensions
    {
        public static bool IsDigit(this char c)
        {
            if (c == '0' || c == '1' || c == '2' || c == '3' || c == '4' || c == '5' || c == '6' || c == '7' || c == '8' ||
                c == '9') return true;
            return false;
        }
        public static bool IsUpperCase(this char c)
        {
            return c >= 'A' && c <= 'Z';
        }

        public static int ToDigit(this char c)
        {
            if (!c.IsDigit()) return 0;
            return Int32.Parse(c.ToString());
        }
    }
}
