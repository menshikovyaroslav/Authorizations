using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Dom.Extensions
{
    public static class StringExtensions
    {
        public static bool ToBoolean(this string x)
        {
            if (String.IsNullOrEmpty(x)) return false;

            x = x.ToLower();
            if (x == "true") return true;
            if (x == "1") return true;
            if (x == "0") return false;

            return false;
        }

        public static string ToStr(this List<string> list)
        {
            var result = string.Empty;
            for (int i = 0; i < list.Count; i++)
            {
                result += list[i];
                if (i != list.Count - 1)
                {
                    result += "; ";
                }
            }

            return result;
        }

        public static Color ToColor(this string x)
        {
            var a1 = x.IndexOf("A=") + 2;
            var a2 = x.IndexOf(",", a1);
            var a = x.Substring(a1, a2 - a1).ToInt();

            var r1 = x.IndexOf("R=") + 2;
            var r2 = x.IndexOf(",", r1);
            var r = x.Substring(r1, r2 - r1).ToInt();

            var g1 = x.IndexOf("G=") + 2;
            var g2 = x.IndexOf(",", g1);
            var g = x.Substring(g1, g2 - g1).ToInt();

            var b1 = x.IndexOf("B=") + 2;
            var b2 = x.IndexOf("]", b1);
            var b = x.Substring(b1, b2 - b1).ToInt();

            return Color.FromArgb(a, r, g, b);
        }

        public static List<int> AllIndexesOf(this string str, string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException("the string to find may not be empty", "value");
            List<int> indexes = new List<int>();
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }

        public static bool IsEmail(this string x)
        {
            //if (String.IsNullOrEmpty(x)) return false;
            //if (!x.Contains("@")) return false;
            //if (!x.Contains(".")) return false;
            //if (x.Length < 6) return false;

            // ^(?(")(".+?(?<!\\)"@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`{}|~\w])*)(?<=[0-9a-z])@))(?([)([(\d{1,3}.){3}\d{1,3}])|(([0-9a-z][-0-9a-z]*[0-9a-z]*.)+[a-z0-9][-a-z0-9]{0,22}[a-z0-9]))$

            return Regex.IsMatch(x, @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$", RegexOptions.IgnoreCase);

            return true;
        }

        public static bool IsContainsCirillic(this string x)
        {
            return Regex.Match(x, "[а-яА-ЯёЁ]").Value.Length > 0;
        }

        public static bool IsDigitsOnly(this string x)
        {
            return Regex.Match(x, @"^\d$").Value.Length <= 0;
        }

        public static int ToInt(this string x)
        {
            if (x.IsEmpty()) return 0;

            var result = 0;
            int.TryParse(x, out result);

            return result;
        }
        public static byte ToByte(this string x)
        {
            byte b;
            byte.TryParse(x, out b);
            return b;
        }
        public static int CharCount(this string x, char c)
        {
            return x.Count(xChar => xChar == c);
        }

        // Последний символ встроке. Строка не должна быть пустой.
        public static char LastSimbol(this string x)
        {
            var len = x.Length;
            return x[len - 1];
        }

        // Пустая ли строка
        public static bool IsEmpty(this string x)
        {
            return string.IsNullOrEmpty(x);
        }
        public static char ToCharSeparator(this string x)
        {
            if (x == @"\t") return '\t';
            if (x == @"|") return '|';
            if (x == @",") return ',';

            return ';';
        }
        public static WebProxy ToWebProxy(this string x)
        {
            if (x.IsEmpty()) return null;

            var split = x.Split(':');
            var ip = split[0];
            var port = 0;
            if (split.Length > 1) int.TryParse(split[1], out port);

            return new WebProxy(ip, port);
        }
        public static string ToBase64(this string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string FromBase64(this string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static bool IsBase64(this string base64String)
        {
            if (string.IsNullOrEmpty(base64String) || base64String.Length % 4 != 0
               || base64String.Contains(" ") || base64String.Contains("\t") || base64String.Contains("\r") || base64String.Contains("\n"))
                return false;
            try
            {
                Convert.FromBase64String(base64String);
                return true;
            }
            catch (Exception exception)
            {
                // Handle the exception
            }
            return false;
        }

        public static string ReplaceWebChars(this string x)
        {
            x = x.Replace("%24", "$");
            x = x.Replace("%25", "%");
            x = x.Replace("%26", "&");

            x = x.Replace("%3A", ":");
            x = x.Replace("%3a", ":");

            x = x.Replace("%2B", "+");
            x = x.Replace("%2b", "+");



            x = x.Replace("%2F", "/");
            x = x.Replace("%2f", "/");

            x = x.Replace("%2E", ".");
            x = x.Replace("%2e", ".");

            x = x.Replace("%3D", "=");

            x = x.Replace("%3F", "?");
            x = x.Replace("%3f", "?");

            x = x.Replace("&amp;", "&");

            return x;
        }
        public static string GetWebParameter(this string x, string parameter)
        {
            var index = x.IndexOf($"{parameter}=");
            if (index < 0) return null;

            // Прибавляем длину параметра и единичку - это длина символа равно
            index += parameter.Length + 1;
            var end = x.IndexOf("&", index);

            // Если это последний параметр строки
            if (end <= 0) return x.Substring(index, x.Length - index);

            return x.Substring(index, end - index);
        }
    }
}
