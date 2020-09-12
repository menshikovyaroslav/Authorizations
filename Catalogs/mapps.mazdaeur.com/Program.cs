using CatalogSupportLibrary;
using CatalogSupportLibrary.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace mapps.mazdaeur.com
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
                Address = "https://mapps.mazdaeur.com/epc3/servlet/LoginEPC",
                Accept = "text/html, application/xhtml+xml, */*",
                Host = "mapps.mazdaeur.com",
                KeepAlive = true,
                Proxy = proxy
            };
            getRequest.Run(ref cookies);

            // find a value of 'lt' parameter in responce
            var nameIndex = getRequest.Response.IndexOf("name=\"lt\"");
            var startIndex1 = nameIndex + 17;
            var endIndex1 = getRequest.Response.IndexOf("\"", startIndex1);
            var lt = getRequest.Response.Substring(startIndex1, endIndex1 - startIndex1);

            // Auth request
            var data = $"username={WebUtility.UrlEncode(user)}&password={WebUtility.UrlEncode(password)}&lt={lt}&execution=e1s1&_eventId=submit";
            var postRequest = new PostRequest()
            {
                Data = data,
                Address = $"https://mapps.mazdaeur.com/cas/login?service=https%3A%2F%2Fmapps.mazdaeur.com%2Fepc3%2Fj_spring_security_check",
                Accept = "text/html, application/xhtml+xml, */*",
                Host = "mapps.mazdaeur.com",
                ContentType = "application/x-www-form-urlencoded",
                Referer = "https://mapps.mazdaeur.com/cas/login?service=https%3A%2F%2Fmapps.mazdaeur.com%2Fepc3%2Fj_spring_security_check",
                KeepAlive = true,
                Proxy = proxy
            };
            postRequest.Run(ref cookies);

            // request to check our status
            getRequest = new GetRequest()
            {
                Address = "https://mapps.mazdaeur.com/epc3/servlet/LoginEPC",
                Accept = "text/html, application/xhtml+xml, */*",
                Host = "mapps.mazdaeur.com",
                KeepAlive = true,
                Proxy = proxy
            };
            getRequest.Run(ref cookies);

            var validIndex = getRequest.Response.Contains("<title>MAZDA WEB-EPC");
            if (validIndex) Config.Instance.AddLogInfo($"Auth result: successful");
            else Config.Instance.AddLogInfo($"Auth result: unknown, something was wrong");
        }
    }
}
