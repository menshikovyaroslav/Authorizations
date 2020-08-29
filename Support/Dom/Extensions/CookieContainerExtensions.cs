using System;
using System.Collections;
using System.Net;
using System.Reflection;

namespace Dom.Extensions
{
    public static class CookieContainerExtensions
    {
        public static CookieCollection GetCookieCollection(this CookieContainer cookieJar)
        {

            CookieCollection cookieCollection = new CookieCollection();

            if (cookieJar == null) return cookieCollection;

            Hashtable table = (Hashtable)cookieJar.GetType().InvokeMember("m_domainTable",
                                                                            BindingFlags.NonPublic |
                                                                            BindingFlags.GetField |
                                                                            BindingFlags.Instance,
                                                                            null,
                                                                            cookieJar,
                                                                            new object[] { });

            foreach (var tableKey in table.Keys)
            {
                String str_tableKey = (string)tableKey;

                if (str_tableKey[0] == '.')
                {
                    str_tableKey = str_tableKey.Substring(1);
                }

                SortedList list = (SortedList)table[tableKey].GetType().InvokeMember("m_list",
                                                                            BindingFlags.NonPublic |
                                                                            BindingFlags.GetField |
                                                                            BindingFlags.Instance,
                                                                            null,
                                                                            table[tableKey],
                                                                            new object[] { });

                foreach (var listKey in list.Keys)
                {
                    String url = "https://" + str_tableKey + (string)listKey;
                    cookieCollection.Add(cookieJar.GetCookies(new Uri(url)));
                }
            }

            return cookieCollection;
        }

        public static Cookie GetCookieByName(this CookieContainer cookieJar, string key)
        {
            var collection = cookieJar.GetCookieCollection();
            foreach (Cookie cookie in collection)
            {
                if (cookie.ToString().Contains(key)) return cookie;
            }
            return null;
        }
        public static void RemoveCookie(this CookieContainer cookieJar, string key, Uri host)
        {
            var cookies = cookieJar.GetCookies(host);
            foreach (Cookie cookie in cookies)
            {
                if (cookie.Name == key)
                {
                    cookie.Expires = DateTime.Now.AddDays(-1);
                    return;
                }
            }
        }
    }
}
