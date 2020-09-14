using CatalogSupportLibrary;
using CatalogSupportLibrary.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace eu.nissan.biz
{
    class Program
    {
        static void Main(string[] args)
        {
            // Each nissan account has two parameters : User, Password
            var user = "user";              // !!! rewrite value from your account !!!
            var password = "password";      // !!! rewrite value from your account !!!

            // We keep cookies here
            var cookies = new CookieContainer();

            // Any proxy, for example Fiddler
            var proxy = new WebProxy("127.0.0.1:8888");

            var getRequest = new GetRequest()
            {
                Address = "https://eu.nissan.biz/",
                Accept = "text/html, application/xhtml+xml, */*",
                Host = "eu.nissan.biz",
                KeepAlive = true,
                Proxy = proxy
            };
            getRequest.Run(ref cookies);

            // Auth request
            var data = $"option=credential&target=https%3A%2F%2Feu.nissan.biz%2F&Ecom_User_ID={WebUtility.UrlEncode(user)}&Ecom_Password={WebUtility.UrlEncode(password)}&submit1=Log+in";
            var postRequest = new PostRequest()
            {
                Data = data,
                Address = $"https://login.eu.nissan.biz/nidp/idff/sso?sid=0&sid=0",
                Accept = "text/html, application/xhtml+xml, */*",
                Host = "login.eu.nissan.biz",
                ContentType = "application/x-www-form-urlencoded",
                Referer = "https://login.eu.nissan.biz/nidp/idff/sso?id=B2B&sid=0&option=credential&sid=0&target=https%3A%2F%2Feu.nissan.biz%2F",
                KeepAlive = true,
                Proxy = proxy
            };
            postRequest.Run(ref cookies);

            // check for a state
            getRequest = new GetRequest()
            {
                Address = "https://eu.nissan.biz",
                Accept = "text/html, application/xhtml+xml, */*",
                Host = "eu.nissan.biz",
                KeepAlive = true,
                TimeOut = 120000,
                Proxy = proxy
            };
            getRequest.Run(ref cookies);

            var validIndex = getRequest.Response.Contains("nissan-user");
            if (validIndex) Config.Instance.AddLogInfo($"Auth result: successful");
            else Config.Instance.AddLogInfo($"Auth result: unknown, something was wrong");
        }
    }
}
