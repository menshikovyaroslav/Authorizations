using CatalogSupportLibrary;
using CatalogSupportLibrary.Requests;
using Dom.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace portal_etc_auto.vaz.ru
{
    class Program
    {
        static void Main(string[] args)
        {
            // Each vaz account has two parameters : User, Password
            var user = "user";              // !!! rewrite value from your account !!!
            var password = "password";      // !!! rewrite value from your account !!!

            // We keep cookies here
            var cookies = new CookieContainer();

            // Any proxy, for example Fiddler
            var proxy = new WebProxy("127.0.0.1:8888");

            // auth request
            var data = $"id2_hf_0=&username={user}&password={password}&search=%D0%92%D0%BE%D0%B9%D1%82%D0%B8";
            var postRequest = new PostRequest()
            {
                Data = data,
                Address = $"http://portal-etc-auto.vaz.ru:7003/login?0-1.IFormSubmitListener-signIn-signInForm",
                Accept = "text/html, application/xhtml+xml, */*",
                Host = "portal-etc-auto.vaz.ru:7003",
                ContentType = "application/x-www-form-urlencoded",
                Referer = "http://portal-etc-auto.vaz.ru:7003/login",
                KeepAlive = true,
                Proxy = proxy
            };
            postRequest.Run(ref cookies);

            // writing auth response in log
            Config.Instance.AddLogInfo($"vaz auth answer: {postRequest.Response}");

            // comfortable cookies presentation
            var catalogCookies = cookies.GetCookieCollection();

            // writing cookies in log
            foreach (var cookie in catalogCookies)
            {
                Config.Instance.AddLogInfo($"cookie: {cookie}");
            }

            var result = "unknown";
            if (postRequest.Response.Contains("news")) result = "Authentication successful";
            else result = "Authentication failed";

            // writing Authentication result status in log
            Config.Instance.AddLogInfo($"Auth result: {result}");
        }
    }
}
