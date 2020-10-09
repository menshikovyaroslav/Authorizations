using CatalogSupportLibrary;
using CatalogSupportLibrary.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace www.parts.renault.com
{
    class Program
    {
        static void Main(string[] args)
        {
            // Each renault account has two parameters : User, Password
            var user = "user";              // !!! rewrite value from your account !!!
            var password = "password";      // !!! rewrite value from your account !!!

            // We keep cookies here
            var cookies = new CookieContainer();

            // Any proxy, for example Fiddler
            var proxy = new WebProxy("127.0.0.1:8888");

            var getRequest = new GetRequest()
            {
                Address = "https://www.parts.renault.com/rpspub/aGetInternetPublicHomePageActionOut.do",
                Accept = "text/html, application/xhtml+xml, */*",
                Host = "www.parts.renault.com",
                KeepAlive = true,
                TimeOut = 60000,
                Proxy = proxy
            };
            getRequest.Run(ref cookies);

            getRequest = new GetRequest()
            {
                Address = "https://www.parts.renault.com/rps/aCreateHomeActionOut.do",
                Accept = "text/html, application/xhtml+xml, */*",
                Host = "www.parts.renault.com",
                TimeOut = 60000,
                KeepAlive = true,
                Referer = "https://www.parts.renault.com/rpspub/aGetInternetPublicHomePageActionOut.do",
                Proxy = proxy
            };
            getRequest.Run(ref cookies);

            var contextIdStart = getRequest.Response.IndexOf("contextId=") + 10;
            var contextIdEnd = getRequest.Response.IndexOf("\"", contextIdStart);
            var contextId = getRequest.Response.Substring(contextIdStart, contextIdEnd - contextIdStart);

            var uidStart = getRequest.Response.IndexOf("name=\"uid\"") + 18;
            var uidEnd = getRequest.Response.IndexOf("\"", uidStart);
            var uid = getRequest.Response.Substring(uidStart, uidEnd - uidStart);

            var postRequest = new PostRequest()
            {
                Data = $"spName=TAI_RPS_PROD_LDAP&uid={uid}&requestedUrl=https%3A%2F%2Fwww.parts.renault.com%2Frps%2FaCreateHomeActionOut.do&logoutUrl=https%3A%2F%2Fwww.parts.renault.com%2Frps%2FaPrepareLogOutActionIn.do&firstRequest=1",
                Address = $"https://www.parts.renault.com/auth/?contextId={contextId}",
                Accept = "text/html, application/xhtml+xml, */*",
                Host = "www.parts.renault.com",
                ContentType = "application/x-www-form-urlencoded",
                TimeOut = 60000,
                Referer = "https://www.parts.renault.com/rps/aCreateHomeActionOut.do",
                KeepAlive = true,
                Proxy = proxy
            };
            postRequest.Run(ref cookies);

            postRequest = new PostRequest()
            {
                Data = $"user={user}&pass={password}&Connexion=Connection",
                Address = $"https://www.parts.renault.com/auth/authentication?contextId={contextId}",
                Accept = "text/html, application/xhtml+xml, */*",
                Host = "www.parts.renault.com",
                ContentType = "application/x-www-form-urlencoded",
                TimeOut = 60000,
                Referer = $"https://www.parts.renault.com/auth/?contextId={contextId}",
                KeepAlive = true,
                Expect100Continue = true,
                Proxy = proxy
            };
            postRequest.Run(ref cookies);


            if (!postRequest.Response.Contains("<title>Redirect to application</title>"))
            {
                // Auth is bad if we are here
                Config.Instance.AddLogInfo($"Auth is failed");
                return;
            }

            var ticketStart = postRequest.Response.IndexOf("name=\"ticket\"") + 21;
            var ticketEnd = postRequest.Response.IndexOf("\"", ticketStart);
            var ticket = postRequest.Response.Substring(ticketStart, ticketEnd - ticketStart);

            var identStart = postRequest.Response.IndexOf("name=\"ident\"") + 20;
            var identEnd = postRequest.Response.IndexOf("\"", identStart);
            var ident = postRequest.Response.Substring(identStart, identEnd - identStart);

            postRequest = new PostRequest()
            {
                Data = $"ticket={ticket}&ident={ident}",
                Address = @"https://www.parts.renault.com/rps/aCreateHomeActionOut.do",
                Accept = "text/html, application/xhtml+xml, */*",
                Host = "www.parts.renault.com",
                ContentType = "application/x-www-form-urlencoded",
                TimeOut = 60000,
                Referer = $"https://www.parts.renault.com/auth/authentication?contextId={contextId}",
                KeepAlive = true,
                Proxy = proxy
            };
            postRequest.Run(ref cookies);

            // Check our session here
            getRequest = new GetRequest()
            {
                Address = $"https://www.parts.renault.com/rps/aCreateHomeActionOut.do",
                Accept = "text/html, application/xhtml+xml, */*",
                Host = "www.parts.renault.com",
                KeepAlive = true,
                TimeOut = 60000,
                Proxy = proxy
            };
            getRequest.Run(ref cookies);

            var result = "unknown";
            if (getRequest.Response.Contains("<title>RENAULTPARTS")) result = "Authentication is successful";
            else result = "Authentication is failed";

            // writing Authentication result status in log
            Config.Instance.AddLogInfo($"Auth result: {result}");
        }
    }
}
