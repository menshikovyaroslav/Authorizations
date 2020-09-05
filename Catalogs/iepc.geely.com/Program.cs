using CatalogSupportLibrary;
using CatalogSupportLibrary.Requests;
using Dom.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace iepc.geely.com
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
                Address = "https://iepc.geely.com/epc/login-input.jspx",
                Accept = "text/html, application/xhtml+xml, image/jxr, */*",
                Host = "iepc.geely.com",
                KeepAlive = true,
                Proxy = proxy
            };
            getRequest.Run(ref cookies);

            // here we get captcha
            getRequest = new GetRequest()
            {
                Address = "https://iepc.geely.com/epc/getRandomValidateCode.jspx",
                Accept = "image/png, image/svg+xml, image/jxr, image/*;q=0.8, */*;q=0.5",
                Host = "iepc.geely.com",
                KeepAlive = true,
                Referer = "https://iepc.geely.com/epc/randomValidateCode.jspx",
                Proxy = proxy
            };
            getRequest.Run(ref cookies);

            // coords of our mouse click on the auth button
            Random random = new Random();
            var x = random.Next(50);
            var y = random.Next(10);

            // we should use any anticaptcha service to take a captcha text
            var captchaText = "";

            // auth request
            var postRequest = new PostRequest()
            {
                Data = $"serialNumber=undefined&username={user}&validate_code={captchaText}&password={password}&.x={x}&.y={y}",
                Address = $"https://iepc.geely.com/epc/Login.jspx",
                Accept = "text/html, application/xhtml+xml, image/jxr, */*",
                Host = "iepc.geely.com",
                ContentType = "application/x-www-form-urlencoded",
                Referer = "https://iepc.geely.com/epc/login-input.jspx",
                KeepAlive = true,
                Proxy = proxy
            };
            postRequest.Run(ref cookies);

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
