using CatalogSupportLibrary;
using CatalogSupportLibrary.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace roblox.com
{
    class Program
    {
        static void Main(string[] args)
        {
            // Each roblox account has two parameters : User, Password
            var user = "user";              // !!! rewrite value from your account !!!
            var password = "password";      // !!! rewrite value from your account !!!

            // We keep cookies here
            var cookies = new CookieContainer();

            // Any proxy, for example Fiddler
            var proxy = new WebProxy("127.0.0.1:8888");

            var getRequest = new GetRequest()
            {
                Address = "https://www.roblox.com/",
                Accept = "text/html, application/xhtml+xml, image/jxr, */*",
                Host = "www.roblox.com",
                KeepAlive = true,
                Proxy = proxy
            };
            getRequest.Run(ref cookies);

            var tokenStart = getRequest.Response.IndexOf("setToken") + 10;
            var tokenEnd = getRequest.Response.IndexOf("'", tokenStart);
            var token = getRequest.Response.Substring(tokenStart, tokenEnd - tokenStart);

            getRequest = new GetRequest()
            {
                Address = "https://www.roblox.com/login",
                Accept = "text/html, application/xhtml+xml, image/jxr, */*",
                Host = "www.roblox.com",
                KeepAlive = true,
                Referer = "https://www.roblox.com/",
                Proxy = proxy
            };
            getRequest.Run(ref cookies);

            // Auth request
            var data = $"{{\"cvalue\":\"{WebUtility.UrlEncode(user)}\",\"ctype\":\"Username\",\"password\":\"{WebUtility.UrlEncode(password)}\"}}";
            var postRequest = new PostRequest()
            {
                Data = data,
                Address = $"https://auth.roblox.com/v2/login",
                Accept = "application/json, text/plain, */*",
                Host = "auth.roblox.com",
                ContentType = "application/json;charset=utf-8",
                Referer = "https://www.roblox.com/login",
                KeepAlive = true,
                Proxy = proxy
            };
            postRequest.AddHeader("X-CSRF-TOKEN", token);
            postRequest.AddHeader("Origin", "https://www.roblox.com");
            postRequest.Run(ref cookies);

            // Use Json parser. I dont want to use other libraries to make my code more difficult.
            var idStart = postRequest.Response.IndexOf("id") + 4;
            var idEnd = postRequest.Response.IndexOf(",", idStart);
            var id = postRequest.Response.Substring(idStart, idEnd - idStart);

            getRequest = new GetRequest()
            {
                Address = "https://web.roblox.com/home?nl=true",
                Accept = "text/html, application/xhtml+xml, image/jxr, */*",
                Host = "www.roblox.com",
                KeepAlive = true,
                Referer = "https://www.roblox.com/login",
                Proxy = proxy
            };
            getRequest.Run(ref cookies);

            getRequest = new GetRequest()
            {
                Address = $"https://economy.roblox.com/v1/users/{id}/currency",
                Accept = "application/json, text/plain, */*",
                Host = "economy.roblox.com",
                KeepAlive = true,
                Referer = "https://web.roblox.com/home?nl=true",
                Proxy = proxy
            };
            getRequest.Run(ref cookies);

            // Write to log the response of robux count request
            Config.Instance.AddLogInfo(getRequest.Response);
        }
    }
}
