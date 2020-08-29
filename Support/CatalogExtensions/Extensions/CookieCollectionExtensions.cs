using System.Collections.Generic;
using System.Linq;
using System.Net;
using Dom.Catalog;

namespace Dom.Extensions
{
    public static class CookieCollectionExtensions
    {
        public static string[] GetArrayOfString(this CookieCollection collection)
        {
            var result = new string[collection.Count];
            for (int i = 0; i < collection.Count; i++)
            {
                result[i] = collection[i].ToString();
            }
            return result;
        }

        public static List<CatalogCookie> CatalogCookies(this CookieCollection collection)
        {
            return (from Cookie cookie in collection select new CatalogCookie(cookie.Name, cookie.Value)).ToList();
        }

        public static List<CatalogCookie> CatalogCookies(this CookieCollection collection, string url)
        {
            var result = new List<CatalogCookie>();
            foreach (Cookie cookie in collection)
            {
                if (cookie.Domain.Contains(url))
                    result.Add(new CatalogCookie(cookie.Name, cookie.Value));
            }

            return result;
        }
    }
}
