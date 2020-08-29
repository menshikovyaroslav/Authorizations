using System;

namespace Dom.Extensions
{
    public static class BoolExtensions
    {
        public static string ToRusStr(this bool b)
        {
            if (b) return "Да";
            return "Нет";
        }
    }
}
