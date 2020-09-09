using CatalogSupportLibrary;
using CatalogSupportLibrary.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace pay.kvartplata.ru
{
    class Program
    {
        static void Main(string[] args)
        {
            // Each kvartplata account has two parameters : User, Password
            var user = "+x (xxx) xxx-xxxx";              // Warning!!! if your login name is your phone number, then field 'user' will look like +7 (xxx) xxx-xxxx  ...i am shocked...
            var password = "password";                    // !!! rewrite value from your account !!!

            // We keep cookies here
            var cookies = new CookieContainer();

            // Any proxy, for example Fiddler
            var proxy = new WebProxy("127.0.0.1:8888");

            var getRequest = new GetRequest()
            {
                Address = "http://pay.kvartplata.ru/",
                Accept = "text/html, application/xhtml+xml, image/jxr, */*",
                Host = "pay.kvartplata.ru",
                KeepAlive = true,
                Proxy = proxy
            };
            getRequest.Run(ref cookies);

            var tokenStart = getRequest.Response.IndexOf("name=\"loginToken") + 25;
            var tokenEnd = getRequest.Response.IndexOf("\"", tokenStart);
            var token = getRequest.Response.Substring(tokenStart, tokenEnd - tokenStart);

            // Auth request
            var data = $"struts.token.name=loginToken&loginToken={token}&userName={WebUtility.UrlEncode(user)}&userPass={WebUtility.UrlEncode(password)}&captchaCode=x&timezone=-420&loginModule=pk";
            var postRequest = new PostRequest()
            {
                Data = data,
                Address = $"https://pay.kvartplata.ru/pk/doLogin!enter.action",
                Accept = "text/html, application/xhtml+xml, */*",
                Host = "pay.kvartplata.ru",
                ContentType = "application/x-www-form-urlencoded",
                Referer = "https://pay.kvartplata.ru/pk/login.action",
                KeepAlive = true,
                Proxy = proxy
            };
            postRequest.Run(ref cookies);

            if (postRequest.Response.Contains("loginForm"))
            {
                Config.Instance.AddLogInfo($"You entered incorrected login or password");
                return;
            }

            getRequest = new GetRequest()
            {
                Address = "https://pay.kvartplata.ru/pk/myServices.action",
                Accept = "text/html, application/xhtml+xml, image/jxr, */*",
                Host = "pay.kvartplata.ru",
                KeepAlive = true,
                Referer = "https://pay.kvartplata.ru/pk/doLogin!enter.action",
                Proxy = proxy
            };
            getRequest.Run(ref cookies);

            var addressStart = getRequest.Response.IndexOf("<tr class=\"grid-group\">") + 25;
            addressStart = getRequest.Response.IndexOf("<td", addressStart);
            addressStart = getRequest.Response.IndexOf(">", addressStart) + 1;

            var addressEnd = getRequest.Response.IndexOf("<", addressStart);
            var address = getRequest.Response.Substring(addressStart, addressEnd - addressStart);

            address = address.Trim(' ', '\t', '\r', '\n');

            Config.Instance.AddLogInfo($"Your address: {address}");

        }
    }
}
