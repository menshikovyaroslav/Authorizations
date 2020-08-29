using CatalogSupportLibrary;
using CatalogSupportLibrary.Requests;
using Dom.Extensions;
using System.Net;

namespace www.ecom.honda_eu.com
{
    class Program
    {
        static void Main(string[] args)
        {
            // Each honda account have four parameters : Country, Business Unit, User, Password
            var country = "country";
            var businessUnit = "businessUnit";
            var user = "user";
            var password = "password";

            // you may know the type of your honda account. for example for my accounts: if (country == 290) then isCorporative = true, if (country == 800) isCorporative = false
            bool isCorporative = true;

            // We keep cookies here
            var cookies = new CookieContainer();

            // Any proxy, for example Fiddler
            var proxy = new WebProxy("127.0.0.1:8888");

            // Creating a first get-request, saving cookies
            var getRequest = new GetRequest()
            {
                Address = "https://www.ecom.honda-eu.com/logon/r_portal.html",
                Accept = "text/html, application/xhtml+xml, */*",
                Host = "www.ecom.honda-eu.com",
                KeepAlive = true,
                Proxy = proxy,
                TimeOut = 60000
            };
            getRequest.Run(ref cookies);

            // using in authorization
            var username = $"{country}-{businessUnit}-{user}";

            PostRequest postRequest;

            // we need to create an additional post-request for noncorporative account
            if (!isCorporative)
            {
                postRequest = new PostRequest()
                {
                    Data = $"URL=&Username={username}&password={password}&requestedURL=%2Flogon%2Fr_portal.html&login-form-type=pwd&txtCountry={country}&txtDealer={businessUnit}&txtUser={user}&txtPassword={password}",
                    Address = $"https://www.ecom.honda-eu.com/indep/independents/logon.jsp",
                    Accept = "text/html, application/xhtml+xml, */*",
                    Host = "www.ecom.honda-eu.com",
                    ContentType = "application/x-www-form-urlencoded",
                    Referer = "https://www.ecom.honda-eu.com/logon/r_portal.html",
                    KeepAlive = true,
                    Proxy = proxy,
                    TimeOut = 60000     // sometimes the portal slows down
                };
                postRequest.Run(ref cookies);
            }

            // different request Data for account type
            var data = isCorporative
                ? $"URL=&Username={username}&password={password}&requestedURL=%2Flogon%2Fr_portal.html&login-form-type=pwd&txtCountry={country}&txtDealer={businessUnit}&txtUser={user}&txtPassword={password}"
                : $"URL=&Username={username}&password={password}&requestedURL=%2Fpanex%2F&login-form-type=pwd";

            // different Referrer for account type
            var referer = isCorporative
                ? "https://www.ecom.honda-eu.com/logon/r_portal.html"
                : "https://www.ecom.honda-eu.com/indep/independents/logon.jsp";

            // creating authorization request
            postRequest = new PostRequest()
            {
                Data = data,
                Address = $"https://www.ecom.honda-eu.com/pkmslogin.form",
                Accept = "text/html, application/xhtml+xml, */*",
                Host = "www.ecom.honda-eu.com",
                ContentType = "application/x-www-form-urlencoded",
                Referer = referer,
                KeepAlive = true,
                Proxy = proxy,
                TimeOut = 60000     // sometimes the portal slows down
            };
            postRequest.Run(ref cookies);

            // writing auth response in log
            Config.Instance.AddLogInfo($"honda auth answer: {postRequest.Response}");

            // comfortable cookies presentation
            var catalogCookies = cookies.GetCookieCollection().CatalogCookies();

            // writing cookies in log
            foreach (var cookie in catalogCookies)
            {
                Config.Instance.AddLogInfo($"cookie: {cookie}");
            }

            var result = "unknown";
            if (postRequest.Response.Contains("Authentication failed")) result = "Authentication failed";
            else if (postRequest.Response.Contains("Honda Login successful")) result =  "Authentication successful";

            // writing Authentication result status in log
            Config.Instance.AddLogInfo($"Auth result: {result}");
        }
    }
}
