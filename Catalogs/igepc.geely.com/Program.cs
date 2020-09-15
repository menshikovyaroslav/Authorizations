using CatalogSupportLibrary;
using CatalogSupportLibrary.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace igepc.geely.com
{
    class Program
    {
        static void Main(string[] args)
        {
            // Each geely account has two parameters : User, Password
            var user = "user";              // !!! rewrite value from your account !!!
            var password = "password";      // !!! rewrite value from your account !!!

            // We keep cookies here
            var cookies = new CookieContainer();

            // Any proxy, for example Fiddler
            var proxy = new WebProxy("127.0.0.1:8888");

            var getRequest = new GetRequest()
            {
                Address = "https://igepc.geely.com/",
                Accept = "text/html, application/xhtml+xml, */*",
                Host = "igepc.geely.com",
                KeepAlive = true,
                Proxy = proxy
            };
            getRequest.Run(ref cookies);

            // here we get a captcha
            getRequest = new GetRequest()
            {
                Address = "https://igepc.geely.com/api/user:captcha",
                Accept = "image/png",
                Host = "igepc.geely.com",
                KeepAlive = true,
                UseUnsafeHeaderParsing = true,
                Referer = "https://igepc.geely.com/",
                Proxy = proxy
            };
            getRequest.Run(ref cookies);

            // we should use any anticaptcha service to take a captcha text
            var captchaText = "";

            // Auth request
            var postRequest = new PostRequest()
            {
                Data = $"{{\"loginName\":\"{WebUtility.UrlEncode(user)}\",\"password\":\"{WebUtility.UrlEncode(password)}\",\"verification\":\"{captchaText}\"}}",
                Address = $"https://igepc.geely.com/api/login:pc?companyId=6",
                Accept = "*/*",
                Host = "igepc.geely.com",
                ContentType = "application/json",
                Referer = "https://igepc.geely.com/",
                KeepAlive = true,
                Proxy = proxy
            };
            postRequest.Run(ref cookies);


            // we find all the new cookies in response headers and add them to cookie container
            var uri = new Uri("http://igepc.geely.com/");
            var headers = postRequest.ResponseHeaders;
            for (int i = 0; i < headers.Count; ++i)
            {
                string header = headers.GetKey(i);
                foreach (string value in headers.GetValues(i))
                {
                    if (header == "Set-Cookie")
                    {
                        var value2 = value.Split(';')[0];
                        var preparedCookie = value2.Split('=');
                        if (preparedCookie.Length < 2) continue;

                        cookies.Add(new Cookie(preparedCookie[0], preparedCookie[1]) { Domain = uri.Host });
                    }
                }
            }

            getRequest = new GetRequest()
            {
                Address = "https://igepc.geely.com/api/onestep/base/epc/language/get_user_language?companyId=6",
                Accept = "*/*",
                Host = "igepc.geely.com",
                KeepAlive = true,
                Referer = "https://igepc.geely.com/",
                Proxy = proxy
            };
            getRequest.Run(ref cookies);

            getRequest = new GetRequest()
            {
                Address = "https://igepc.geely.com/api/Menu/platformId/4?companyId=6",
                Accept = "*/*",
                Host = "igepc.geely.com",
                KeepAlive = true,
                Referer = "https://igepc.geely.com/",
                Proxy = proxy
            };
            getRequest.Run(ref cookies);

            var validIndex = getRequest.Response.Contains("code\":\"success");
            if (validIndex) Config.Instance.AddLogInfo($"Auth result: successful");
            else Config.Instance.AddLogInfo($"Auth result: unknown, something was wrong");
        }
    }
}
