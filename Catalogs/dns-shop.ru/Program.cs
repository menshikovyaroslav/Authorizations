using CatalogSupportLibrary;
using CatalogSupportLibrary.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace dns_shop.ru
{
    class Program
    {
        static void Main(string[] args)
        {
            // Each dns account has two parameters: User, Password
            var user = "user";              // !!! rewrite value from your account !!! Example: 7913xxxxxxx
            var password = "password";      // !!! rewrite value from your account !!!

            // We keep cookies here
            var cookies = new CookieContainer();

            // Any proxy, for example Fiddler
            var proxy = new WebProxy("127.0.0.1:8888");

            var getRequest = new GetRequest()
            {
                Address = "https://www.dns-shop.ru/",
                Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9",
                Host = "www.dns-shop.ru",
                KeepAlive = true,
                Proxy = proxy
            };
            getRequest.Run(ref cookies);

            var tokenStart = getRequest.Response.IndexOf("csrf-token") + 21;
            var tokenEnd = getRequest.Response.IndexOf("\"", tokenStart);
            var token = getRequest.Response.Substring(tokenStart, tokenEnd - tokenStart);

            // This parameter differs in different browsers, but i didn't see the problem with it
            var key = "------WebKitFormBoundaryazod1ayqWxHQOUrv";

            // Auth request
            var postRequest = new PostRequest()
            {
                Data = $"{key}\r\n" +
                        "Content-Disposition: form-data; name=\"LoginPasswordAuthorizationLoadForm[login]\"\r\n" +
                        "\r\n" +
                        $"{user}\r\n" +
                        $"{key}\r\n" +
                        "Content-Disposition: form-data; name=\"LoginPasswordAuthorizationLoadForm[password]\"\r\n" +
                        "\r\n" +
                        $"{password}\r\n" +
                        $"{key}\r\n" +
                        "Content-Disposition: form-data; name=\"LoginPasswordAuthorizationLoadForm[token]\"\r\n" +
                        "\r\n" +
                        "\r\n" +
                        $"{key}\r\n",
                Address = $"https://www.dns-shop.ru/auth/auth/login-password-authorization/",
                Accept = "*/*",
                ContentType = $"multipart/form-data; boundary={key}",
                Host = "www.dns-shop.ru",
                Referer = "https://www.dns-shop.ru/no-referrer",
                KeepAlive = true,
                Proxy = proxy
            };
            postRequest.AddHeader("X-Requested-With", "XMLHttpRequest");
            postRequest.AddHeader("X-CSRF-Token", token);
            postRequest.AddHeader("Origin", "https://www.dns-shop.ru");
            postRequest.AddHeader("Sec-Fetch-Dest", "empty");
            postRequest.AddHeader("Sec-Fetch-Mode", "cors");
            postRequest.AddHeader("Sec-Fetch-Site", "same-origin");
            postRequest.Run(ref cookies);

            var validIndex = postRequest.Response.Contains("\"result\":true");
            if (validIndex) Config.Instance.AddLogInfo($"Auth result: successful");
            else Config.Instance.AddLogInfo($"Auth result: unknown, something was wrong");
        }
    }
}
