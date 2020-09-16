using CatalogSupportLibrary;
using CatalogSupportLibrary.Requests;
using Dom.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ngs.ru
{
    class Program
    {
        static void Main(string[] args)
        {
            // Each ngs account has two parameters : User, Password
            var user = "user";              // !!! rewrite value from your account !!!
            var password = "password";      // !!! rewrite value from your account !!!

            // We keep cookies here
            var cookies = new CookieContainer();

            // Any proxy, for example Fiddler
            var proxy = new WebProxy("127.0.0.1:8888");

            // This portal uses basic authorization:

            // We need to create auth string
            var authString = $"{user}:{password}".ToBase64();

            var getRequest = new GetRequest()
            {
                Address = "https://ngs.ru/",
                Accept = "text/html, application/xhtml+xml, image/jxr, */*",
                Host = "ngs.ru",
                KeepAlive = true,
                Proxy = proxy
            };
            getRequest.AddHeader("Authorization", $"Basic {authString}");
            getRequest.Run(ref cookies);

            // Auth request doesn't need, only adding an auth header
            var postRequest = new PostRequest()
            {
                Data = "",
                Address = $"https://newsapi.ngs.ru/v1/public/account/?regionId=54",
                Accept = "vnd.news.v1.jtnews+json",
                Host = "newsapi.ngs.ru",
                Referer = "https://ngs.ru/",
                KeepAlive = true,
                Proxy = proxy
            };
            postRequest.AddHeader("Origin", "https://ngs.ru");
            postRequest.AddHeader("Authorization", $"Basic {authString}");
            postRequest.Run(ref cookies);

            var validIndex = getRequest.Response.Contains("userId\":");
            if (validIndex) Config.Instance.AddLogInfo($"Auth result: successful");
            else Config.Instance.AddLogInfo($"Auth result: unknown, something was wrong");
        }
    }
}
