using CatalogSupportLibrary;
using CatalogSupportLibrary.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace invest_idei.ru
{
    class Program
    {
        static void Main(string[] args)
        {
            // Each invest-idei.ru account has two parameters : User, Password
            var user = "user";              // !!! rewrite value from your account !!!
            var password = "password";      // !!! rewrite value from your account !!!

            // We keep cookies here
            var cookies = new CookieContainer();

            // Any proxy, for example Fiddler
            var proxy = new WebProxy("127.0.0.1:8888");

            var getRequest = new GetRequest()
            {
                Address = "https://invest-idei.ru/",
                Accept = "text/html, application/xhtml+xml, */*",
                Host = "invest-idei.ru",
                Proxy = proxy,
                KeepAlive = true
            };
            getRequest.Run(ref cookies);

            try
            {
                var postRequest = new PostRequest()
                {
                    Data = $"{{\"email\":\"{user}\",\"password\":\"{password}\"}}",
                    Address = $"https://invest-idei.ru/auth/login",
                    Accept = "application/json, text/plain, */*",
                    ContentType = "application/json;charset=UTF-8",
                    Host = "invest-idei.ru",
                    Referer = "https://invest-idei.ru/auth/login",
                    KeepAlive = true,
                    Proxy = proxy
                };
                postRequest.AddHeader("X-Requested-With", "XMLHttpRequest");
                postRequest.AddHeader("Sec-Fetch-Dest", "empty");
                postRequest.AddHeader("Origin", "https://invest-idei.ru");
                postRequest.AddHeader("Sec-Fetch-Mode", "cors");
                postRequest.AddHeader("Sec-Fetch-Site", "same-origin");
                postRequest.Run(ref cookies);

                if (postRequest.Response.Contains("{\"result\":\"ok\"}"))
                {
                    Config.Instance.AddLogInfo($"Auth result: ok");
                }
            }
            catch (Exception ex)
            {
                Config.Instance.AddLogInfo($"Auth result: bad, {ex.Message}");
            }
        }
    }
}
