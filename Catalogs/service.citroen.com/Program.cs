using CatalogSupportLibrary;
using CatalogSupportLibrary.Requests;
using Dom.Extensions;
using System.Net;

namespace service.citroen.com
{
    class Program
    {
        static void Main(string[] args)
        {
            // Each citroen account has two parameters : User, Password
            var user = "user";              // !!! rewrite value from your account !!!
            var password = "password";      // !!! rewrite value from your account !!!

            // We keep cookies here
            var cookies = new CookieContainer();

            // Any proxy, for example Fiddler
            var proxy = new WebProxy("127.0.0.1:8888");

            // we need to create this request to get 'windowName' parameter. it requires for auth
            var getRequest = new GetRequest()
            {
                Address = "http://service.citroen.com/pages/index.jsp",
                Accept = "text/html, application/xhtml+xml, */*",
                Host = "service.citroen.com",
                KeepAlive = true,
                TimeOut = 10000,
                Proxy = proxy
            };
            getRequest.Run(ref cookies);

            // find 'windowName' parameter
            var startIndex = getRequest.Response.IndexOf("window.name") + 14;
            var endIndex = getRequest.Response.IndexOf("'", startIndex);
            var windowName = getRequest.Response.Substring(startIndex, endIndex - startIndex);

            // auth request
            var postRequest = new PostRequest()
            {
                Data = $"userid={user}&password={password}&window={windowName}",
                Address = @"http://service.citroen.com/do/login",
                Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8",
                Host = "service.citroen.com",
                ContentType = "application/x-www-form-urlencoded",
                Referer = "http://service.citroen.com/pages/index.jsp",
                KeepAlive = true,
                Proxy = proxy
            };
            postRequest.Run(ref cookies);

            // writing auth response in log
            Config.Instance.AddLogInfo($"citroen auth answer: {postRequest.Response}");

            getRequest = new GetRequest()
            {
                Address = "http://service.citroen.com/do/newApvprStart",
                Accept = "text/html, application/xhtml+xml, */*",
                Host = "service.citroen.com",
                KeepAlive = true,
                Proxy = proxy,
                Referer = "http://service.citroen.com/pages/index.jsp"
            };
            getRequest.Run(ref cookies);

            // comfortable cookies presentation
            var catalogCookies = cookies.GetCookieCollection().CatalogCookies();

            // writing cookies in log
            foreach (var cookie in catalogCookies)
            {
                Config.Instance.AddLogInfo($"cookie: {cookie}");
            }

            var result = "unknown";
            if (postRequest.Response.Contains("loginForm")) result = "Authentication failed";
            else result = "Authentication successful";

            // writing Authentication result status in log
            Config.Instance.AddLogInfo($"Auth result: {result}");
        }
    }
}
