using System;

namespace Dom.Extensions
{
    public static class IntExtensions
    {
        public static int RoundTo10(this int x)
        {
            double dx = x;
            return (int)(Math.Round(dx / 10) * 10);
        }

        public static int AddProcents(this int x, double procent)
        {
            return (int)(x + (x*procent)/100);
        }

        public static int GetDigitCount(this int x)
        {
            return x.ToString().Length;
        }

        public static bool IsEven(this int x)
        {
            if (x%2 == 0) return true;
            return false;
        }
    }
}
