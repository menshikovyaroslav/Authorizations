using System.Net;
using Dom.Catalog;

namespace Dom.Extensions
{
    public static class CookieExtensions
    {
        public static CatalogCookie CatalogCookie(this Cookie cookie)
        {
            return new CatalogCookie(cookie.Name, cookie.Value);
        }
    }
}
