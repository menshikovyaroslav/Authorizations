using CatalogSupportLibrary;
using CatalogSupportLibrary.Requests;
using Dom.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace eportal.daf.com
{
    class Program
    {
        static void Main(string[] args)
        {
            // Each daf account has two parameters : User, Password
            var user = "user";              // !!! rewrite value from your account !!!
            var password = "password";      // !!! rewrite value from your account !!!

            // We keep cookies here
            var cookies = new CookieContainer();

            // Any proxy, for example Fiddler
            var proxy = new WebProxy("127.0.0.1:8888");

            var getRequest = new GetRequest()
            {
                Address = "https://eportal.daf.com/",
                Accept = "text/html, application/xhtml+xml, */*",
                Host = "eportal.daf.com",
                KeepAlive = true,
                Proxy = proxy
            };
            getRequest.Run(ref cookies);

            // Auth request
            var data = $"curl=Z2F&flags=0&forcedownlevel=0&formdir=1&trusted=0&username={WebUtility.UrlEncode(user)}&password={WebUtility.UrlEncode(password)}&SubmitCreds=%D0%92%D1%85%D0%BE%D0%B4+%D0%B2+%D1%81%D0%B8%D1%81%D1%82%D0%B5%D0%BC%D1%83";
            var postRequest = new PostRequest()
            {
                Data = data,
                Address = $"https://eportal.daf.com/CookieAuth.dll?Logon",
                Accept = "text/html, application/xhtml+xml, */*",
                Host = "eportal.daf.com",
                ContentType = "application/x-www-form-urlencoded",
                Referer = "https://eportal.daf.com/CookieAuth.dll?GetLogon?curl=Z2F&reason=0&formdir=1",
                KeepAlive = true,
                Proxy = proxy
            };
            postRequest.Run(ref cookies);

            getRequest = new GetRequest()
            {
                Address = "https://eportal.daf.com/",
                Accept = "text/html, application/xhtml+xml, */*",
                Host = "eportal.daf.com",
                KeepAlive = true,
                Referer = "https://eportal.daf.com/CookieAuth.dll?GetLogon?curl=Z2F&reason=0&formdir=1",
                Proxy = proxy
            };
            getRequest.Run(ref cookies);

            getRequest = new GetRequest()
            {
                Address = "https://eportal.daf.com/ePortalMenu/default.aspx",
                Accept = "text/html, application/xhtml+xml, */*",
                Host = "eportal.daf.com",
                KeepAlive = true,
                Proxy = proxy
            };
            getRequest.Run(ref cookies);

            var validIndex = getRequest.Response.IndexOf("<title>DAF ePortal</title>");
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
