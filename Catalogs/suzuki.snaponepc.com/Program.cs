using CatalogSupportLibrary;
using CatalogSupportLibrary.Requests;
using Dom.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace suzuki.snaponepc.com
{
    class Program
    {
        static void Main(string[] args)
        {
            // Each suzuki account have two parameters : User, Password
            var user = "user";              // !!! rewrite value from your account !!!
            var password = "password";      // !!! rewrite value from your account !!!

            // We keep cookies here
            var cookies = new CookieContainer();

            // Any proxy, for example Fiddler
            var proxy = new WebProxy("127.0.0.1:8888");

            var getRequest = new GetRequest()
            {
                Address = "https://suzuki.snaponepc.com/epc/#/",
                Accept = "text/html, application/xhtml+xml, */*",
                Host = "suzuki.snaponepc.com",
                KeepAlive = true,
                Proxy = proxy
            };
            getRequest.Run(ref cookies);

            // password must be encoded into base64
            var passBase64 = password.ToBase64();
            var data = $"user={user}&password={passBase64}";

            // auth request
            var postRequest = new PostRequest()
            {
                Data = data,
                Address = $"https://suzuki.snaponepc.com/epc-services/auth/login/",
                Accept = "text/html, application/xhtml+xml, */*",
                Host = "suzuki.snaponepc.com",
                ContentType = "application/x-www-form-urlencoded",
                Referer = "https://suzuki.snaponepc.com/epc/#/",
                KeepAlive = true,
                Proxy = proxy
            };
            postRequest.Run(ref cookies);

            // we need to use try/catch. if auth is unsuccessful then we will catch an exception
            try
            {
                postRequest = new PostRequest()
                {
                    Data = "",
                    Address = $"https://suzuki.snaponepc.com/epc-services/auth/account",
                    Accept = "text/html, application/xhtml+xml, */*",
                    Host = "suzuki.snaponepc.com",
                    ContentType = "application/x-www-form-urlencoded",
                    Referer = "https://suzuki.snaponepc.com/epc/",
                    KeepAlive = true,
                    Proxy = proxy
                };
                postRequest.Run(ref cookies);
            }
            catch (Exception exception)
            {
                Config.Instance.AddLogInfo($"Auth result: bad. {exception.Message}");
                return;
            }

           

            // if we here then auth is successful

            // comfortable cookies presentation
            var catalogCookies = cookies.GetCookieCollection().CatalogCookies();

            // writing cookies in log
            foreach (var cookie in catalogCookies)
            {
                Config.Instance.AddLogInfo($"cookie: {cookie}");
            }

            // writing Authentication result status in log
            Config.Instance.AddLogInfo($"Auth result: successful");

        }
    }
}
