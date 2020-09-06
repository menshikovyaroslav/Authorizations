using CatalogSupportLibrary;
using CatalogSupportLibrary.Requests;
using Dom.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace drom.ru
{
    class Program
    {
        static void Main(string[] args)
        {
            // Each drom.ru account has two parameters : User, Password
            var user = "user";              // !!! rewrite value from your account !!!
            var password = "password";      // !!! rewrite value from your account !!!

            // We keep cookies here
            var cookies = new CookieContainer();

            // Any proxy, for example Fiddler
            var proxy = new WebProxy("127.0.0.1:8888");

            var getRequest = new GetRequest()
            {
                Address = "https://www.drom.ru/",
                Accept = "text/html, application/xhtml+xml, image/jxr, */*",
                Host = "www.drom.ru",
                KeepAlive = true,
                Proxy = proxy
            };
            getRequest.Run(ref cookies);

            var time = DateTime.Now.ToUnixTime();

            // here we get captcha
            getRequest = new GetRequest()
            {
                Address = $"https://my.drom.ru/sign?return=https%3A%2F%2Fwww.drom.ru%2F%3Ftcb%3D{time}",
                Accept = "text/html, application/xhtml+xml, image/jxr, */*",
                Host = "my.drom.ru",
                KeepAlive = true,
                Referer = "https://www.drom.ru/",
                Proxy = proxy
            };
            getRequest.Run(ref cookies);

            // auth request
            var postRequest = new PostRequest()
            {
                Data = $"radio=sign&sign={user}&password={password}",
                Address = $"https://my.drom.ru/sign?return=https%3A%2F%2Fwww.drom.ru%2F%3Ftcb%3D{time}&login_by_password=1&login_by_password=1",
                Accept = "text/html, application/xhtml+xml, image/jxr, */*",
                Host = "my.drom.ru",
                ContentType = "application/x-www-form-urlencoded",
                Referer = "https://my.drom.ru/sign?return=https%3A%2F%2Fwww.drom.ru%2F%3Ftcb%3D{time}&login_by_password=1",
                KeepAlive = true,
                ResponseEncoding = "Windows-1251",
                Proxy = proxy
            };
            postRequest.Run(ref cookies);

            var result = "unknown";
            if (postRequest.Response.Contains("<title>Дром")) result = "Authentication successful";
            else if (postRequest.Response.Contains("<title>Вход")) result = "Authentication failed";

            // writing Authentication result status in log
            Config.Instance.AddLogInfo($"Auth result: {result}");
        }
    }
}
