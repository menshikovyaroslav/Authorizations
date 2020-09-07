using CatalogSupportLibrary;
using CatalogSupportLibrary.Requests;
using Dom.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace etk_b2i.bmwgroup.com
{
    class Program
    {
        static void Main(string[] args)
        {
            // Each bmw account has two parameters : User, Password
            var user = "user";              // !!! rewrite value from your account !!!
            var password = "password";      // !!! rewrite value from your account !!!

            // We keep cookies here
            var cookies = new CookieContainer();

            // Any proxy, for example Fiddler
            var proxy = new WebProxy("127.0.0.1:8888");

            var getRequest = new GetRequest()
            {
                Address = "https://etk-b2i.bmwgroup.com/?portal=OSMC&lang=ru_RU",
                Accept = "text/html, application/xhtml+xml, */*",
                Host = "etk-b2i.bmwgroup.com",
                AllowAutoRedirect = false,
                KeepAlive = true,
                Proxy = proxy
            };
            getRequest.Run(ref cookies);

            // we should find 'Location' parameter in response headers
            var location = "";
            var keys = getRequest.ResponseHeaders.AllKeys;
            foreach (string key in keys)
            {
                if (key == "Location") location = getRequest.ResponseHeaders.Get(key);
            }

            var smagentnameStart = location.IndexOf("SMAGENTNAME=") + 12;
            var smagentnameEnd = location.IndexOf("&", smagentnameStart);
            var smagentname = location.Substring(smagentnameStart, smagentnameEnd - smagentnameStart);
            var smSplitted = smagentname.Split('$');
            smagentname = smSplitted[smSplitted.Length - 1];

            var typeStart = location.IndexOf("TYPE=") + 5;
            var typeEnd = location.IndexOf("&", typeStart);
            var type = location.Substring(typeStart, typeEnd - typeStart);

            var realmoidStart = location.IndexOf("REALMOID=") + 9;
            var realmoidEnd = location.IndexOf("&", realmoidStart);
            var realmoid = location.Substring(realmoidStart, realmoidEnd - realmoidStart);

            getRequest = new GetRequest()
            {
                Address = location,
                Accept = "text/html, application/xhtml+xml, */*",
                Host = "aos.bmwgroup.com",
                KeepAlive = true,
                Proxy = proxy,
            };
            getRequest.Run(ref cookies);

            // Auth request
            var postRequest = new PostRequest()
            {
                Data = $"target=HTTPS%3A%2F%2Fetk-b2i.bmwgroup.com%2F%3Fportal%3DOSMC%26lang%3Dru_RU&smauthreason=0&smagentname={smagentname}&smtryno=&USER={WebUtility.UrlEncode(user)}&PASSWORD={WebUtility.UrlEncode(password)}",
                Address = location,
                Accept = "text/html, application/xhtml+xml, */*",
                Host = "aos.bmwgroup.com",
                ContentType = "application/x-www-form-urlencoded",
                Referer = location,
                Proxy = proxy,
                KeepAlive = true
            };
            postRequest.Run(ref cookies);

            getRequest = new GetRequest()
            {
                Address = $"https://etk-b2i.bmwgroup.com/etk-rest/api/version?_dc={DateTime.Now.ToUnixTime()}333",  // Added milliseconds at the end
                Accept = "*/*",
                Host = "etk-b2i.bmwgroup.com",
                AllowAutoRedirect = false,
                KeepAlive = true,
                Proxy = proxy,
                Referer = "https://etk-b2i.bmwgroup.com/?portal=OSMC&lang=ru_RU"
            };
            getRequest.Run(ref cookies);

            // Lets check our state
            getRequest = new GetRequest()
            {
                Address = "https://etk-b2i.bmwgroup.com/?portal=OSMC&lang=ru_RU",
                Accept = "text/html, application/xhtml+xml, */*",
                Host = "etk-b2i.bmwgroup.com",
                KeepAlive = true,
                Proxy = proxy
            };
            getRequest.Run(ref cookies);

            var validIndex = getRequest.Response.IndexOf("<title>ETK</title>");
            if (validIndex >= 0) Config.Instance.AddLogInfo($"Auth result: successful");
            else
            {
                Config.Instance.AddLogInfo($"Auth result: unknown, something was wrong");
                return;
            }

            // comfortable cookies presentation
            var catalogCookies = cookies.GetCookieCollection().CatalogCookies();

            // writing cookies in log
            foreach (var cookie in catalogCookies)
            {
                Config.Instance.AddLogInfo($"cookie: {cookie}");
            }
        }
    }
}
