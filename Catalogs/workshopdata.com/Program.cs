using CatalogSupportLibrary;
using CatalogSupportLibrary.Requests;
using Dom.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace workshopdata.com
{
    class Program
    {
        static void Main(string[] args)
        {
            // Each workshop account has two parameters : User, Password
            var user = "user";              // !!! rewrite value from your account !!!
            var password = "password";      // !!! rewrite value from your account !!!

            // We keep cookies here
            var cookies = new CookieContainer();

            // Any proxy, for example Fiddler
            var proxy = new WebProxy("127.0.0.1:8888");

            var getRequest = new GetRequest()
            {
                Address = "https://workshopdata.com/touch/site/layout/wsdLogin?lang=ru",
                Accept = "text/html, application/xhtml+xml, */*",
                Host = "workshopdata.com",
                KeepAlive = true,
                Proxy = proxy
            };
            getRequest.Run(ref cookies);

            // Auth request to kia portal
            var postRequest = new PostRequest()
            {
                Data = $"username={user}&password={password}&submit=%D0%92%D1%85%D0%BE%D0%B4&=",
                Address = $"https://workshopdata.com/touch/site/layout/wsdLogin",
                Accept = "text/html, application/xhtml+xml, */*",
                Host = "workshopdata.com",
                ContentType = "application/x-www-form-urlencoded",
                Referer = $"https://workshopdata.com/touch/site/layout/wsdLogin?lang=ru",
                KeepAlive = true,
                Proxy = proxy
            };
            postRequest.Run(ref cookies);

            getRequest = new GetRequest()
            {
                Address = "https://workshopdata.com/touch/site/layout/makesOverview?urlDownloadSvgViewer=http%3A%2F%2Fdownload.adobe.com%2Fpub%2Fadobe%2Fmagic%2Fsvgviewer%2Fwin%2F3.x%2F3.03%2Fru%2FSVGView.exe",
                Accept = "text/html, application/xhtml+xml, */*",
                Host = "workshopdata.com",
                KeepAlive = true,
                Proxy = proxy
            };
            getRequest.Run(ref cookies);

            Config.Instance.AddLogInfo($"x: {getRequest.Response}");

            // comfortable cookies presentation
            var catalogCookies = cookies.GetCookieCollection().CatalogCookies();

            // writing cookies in log
            foreach (var cookie in catalogCookies)
            {
                Config.Instance.AddLogInfo($"cookie: {cookie}");
            }

            // SUZUKI - is part of all the presented brends here
            var validIndex = getRequest.Response.IndexOf("SUZUKI");
            if (validIndex >= 0) Config.Instance.AddLogInfo($"Auth result: successful");
            else
            {
                Config.Instance.AddLogInfo($"Auth result: unknown, something was wrong");
                return;
            }
        }
    }
}
