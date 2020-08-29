using CatalogSupportLibrary;
using CatalogSupportLibrary.Requests;
using Dom.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace mail.ru
{
    class Program
    {
        static void Main(string[] args)
        {
            // Each citroen account have two parameters : User, Password
            var user = "user";              // !!! rewrite value from your account !!!
            var password = "password";      // !!! rewrite value from your account !!!

            // We keep cookies here
            var cookies = new CookieContainer();

            // Any proxy, for example Fiddler
            var proxy = new WebProxy("127.0.0.1:8888");

            // we need to create this request to get 'windowName' parameter. it requires for auth
            var getRequest = new GetRequest()
            {
                Address = "https://mail.ru/",
                Accept = "text/html, application/xhtml+xml, image/jxr, */*",
                Host = "mail.ru",
                KeepAlive = true,
                TimeOut = 10000,
                Proxy = proxy
            };
            getRequest.Run(ref cookies);

            // find 'token' parameter
            var startIndex = getRequest.Response.IndexOf("CSRF:") + 6;
            var endIndex = getRequest.Response.IndexOf("\"", startIndex);
            var token = getRequest.Response.Substring(startIndex, endIndex - startIndex);

            // auth request
            var postRequest = new PostRequest()
            {
                Data = $"login={WebUtility.UrlEncode(user)}&password={WebUtility.UrlEncode(password)}&saveauth=1&token={token}&project=e.mail.ru&_={DateTime.Now.ToUnixTime()}001",
                Address = @"https://auth.mail.ru/jsapi/auth",
                Accept = "*/*",
                Host = "auth.mail.ru",
                ContentType = "application/x-www-form-urlencoded",
                Referer = "https://mail.ru/",
                KeepAlive = true,
                Proxy = proxy
            };
            postRequest.Run(ref cookies);

            if (postRequest.Response.Contains("{\"status\":\"ok\"}")) Config.Instance.AddLogInfo($"Auth result: ok");
            else
            {
                Config.Instance.AddLogInfo($"Auth result: {postRequest.Response}");
                return;
            }

            // now we can receive something, for example count of messages in the mailbox
            getRequest = new GetRequest()
            {
                Address = $"https://portal.mail.ru/NaviData?mac=1&gamescnt=1&Socials=1&Login={user}&rnd={DateTime.Now.ToUnixTime()}001",
                Accept = "*/*",
                Host = "portal.mail.ru",
                KeepAlive = true,
                TimeOut = 10000,
                Referer = "https://e.mail.ru/messages/inbox/?back=1&afterReload=1",
                Proxy = proxy
            };
            getRequest.Run(ref cookies);

            // find count of messages in the mailbox
            startIndex = getRequest.Response.IndexOf("mail_cnt") + 11;
            endIndex = getRequest.Response.IndexOf("\"", startIndex);
            var count = getRequest.Response.Substring(startIndex, endIndex - startIndex);

            Config.Instance.AddLogInfo($"we have {count} messages in our mailbox");
        }
    }
}
