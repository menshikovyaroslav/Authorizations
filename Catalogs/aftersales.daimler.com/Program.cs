using CatalogSupportLibrary;
using CatalogSupportLibrary.Requests;
using Dom.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace aftersales.daimler.com
{
    class Program
    {
        static void Main(string[] args)
        {
            // Each mercedes account has two parameters : User, Password
            var user = "user";              // !!! rewrite value from your account !!!
            var password = "password";      // !!! rewrite value from your account !!!

            // We keep cookies here
            var cookies = new CookieContainer();

            // Any proxy, for example Fiddler
            var proxy = new WebProxy("127.0.0.1:8888");

            var getRequest = new GetRequest()
            {
                Address = "https://aftersales.daimler.com/login",
                Accept = "text/html, application/xhtml+xml, image/jxr, */*",
                Host = "aftersales.daimler.com",
                KeepAlive = true,
                Proxy = proxy
            };
            getRequest.Run(ref cookies);

            #region a few needed parameters

            var csrfStart = getRequest.Response.IndexOf("_csrf") + 14;
            var csrfEnd = getRequest.Response.IndexOf("\"", csrfStart);
            var csrf = getRequest.Response.Substring(csrfStart, csrfEnd - csrfStart);

            var startPing = getRequest.Response.IndexOf("pingResume=") + 15;
            var endPing = getRequest.Response.IndexOf("/", startPing);
            var ping = getRequest.Response.Substring(startPing, endPing - startPing);

            #endregion

            var uri = new Uri("https://daimler.com");
            cookies.Add(new Cookie("ADRUM", $"s={DateTime.Now.ToUnixTime()}124&r=https%3A%2F%2Flogin.daimler.com%2F%3F{DateTime.Now.ToUnixTime()}") { Domain = uri.Host });

            // auth request
            var postRequest = new PostRequest()
            {
                Data = $"_csrf={csrf}&username={WebUtility.UrlEncode(user)}&pingResume=%2Fas%2F{ping}%2Fresume%2Fas%2Fauthorization.ping&password={WebUtility.UrlEncode(password)}",
                Address = $"https://login.daimler.com/",
                Accept = "text/html, application/xhtml+xml, image/jxr, */*",
                Host = "login.daimler.com",
                ContentType = "application/x-www-form-urlencoded",
                Referer = $"https://login.daimler.com/?brand=default&resume=%2Fas%2F{ping}%2Fresume%2Fas%2Fauthorization.ping&spentity=null",
                KeepAlive = true,
                Proxy = proxy
            };
            postRequest.Run(ref cookies);

            getRequest = new GetRequest()
            {
                Address = "https://aftersales.i.daimler.com/mb-webparts/",
                Accept = "text/html, application/xhtml+xml, */*",
                Host = "aftersales.i.daimler.com",
                KeepAlive = true,
                Referer = "https://aftersales.daimler.com/home/",
                AllowAutoRedirect = false,
                Proxy = proxy
            };
            getRequest.Run(ref cookies);

            #region a few needed parameters

            var s1 = getRequest.Response.IndexOf("&client_id=") + 11;
            var e1 = getRequest.Response.IndexOf("'", s1);
            var clientId = getRequest.Response.Substring(s1, e1 - s1);

            var s2 = getRequest.Response.IndexOf("var csrfValue =") + 17;
            var e2 = getRequest.Response.IndexOf("'", s2);
            var state = getRequest.Response.Substring(s2, e2 - s2);

            #endregion

            var addr = $"https://sso.daimler.com/as/authorization.oauth2?response_type=code&client_id={clientId}&scope=openid%20email%20profile&state={state}&redirect_uri=https%3A%2F%2Faftersales.i.daimler.com%2Fmb-webparts-oidcclient1%2Fsignin_cb";

            getRequest = new GetRequest()
            {
                Address = addr,
                Accept = "text/html, application/xhtml+xml, image/jxr, */*",
                Host = "sso.daimler.com",
                KeepAlive = true,
                Referer = "https://aftersales.i.daimler.com/mb-webparts/",
                Proxy = proxy
            };
            getRequest.Run(ref cookies);

            postRequest = new PostRequest()
            {
                Data = $"clientTimeZone=Asia/Bangkok&browserType=LT_IE9",
                Address = $"https://aftersales.i.daimler.com/mb-webparts/Login.action",
                Accept = "text/html, application/xhtml+xml, image/jxr, */*",
                Host = "aftersales.i.daimler.com",
                ContentType = "application/x-www-form-urlencoded",
                Referer = $"https://aftersales.i.daimler.com/mb-webparts/preLogin.action",
                KeepAlive = true,
                Proxy = proxy
            };
            postRequest.Run(ref cookies);

            var loginTokenStart = postRequest.Response.IndexOf("loginToken=") + 11;
            var loginTokenEnd = postRequest.Response.IndexOf("\"", loginTokenStart);
            var loginToken = postRequest.Response.Substring(loginTokenStart, loginTokenEnd - loginTokenStart);

            getRequest = new GetRequest()
            {
                Address = $"https://aftersales.i.daimler.com/mb-webparts/ng/customer?loginToken={loginToken}&startWizard=true",
                Accept = "text/html, application/xhtml+xml, image/jxr, */*",
                Host = "aftersales.i.daimler.com",
                Referer = $"https://aftersales.i.daimler.com/mb-webparts/ng/customer?0&loginToken={loginToken}",
                KeepAlive = true,
                Proxy = proxy
            };
            getRequest.Run(ref cookies);

            getRequest = new GetRequest()
            {
                Address = "https://aftersales.i.daimler.com/mb-webparts/ng/customer/epc/catalogue",
                Accept = "text/html, application/xhtml+xml, image/jxr, */*",
                Host = "aftersales.i.daimler.com",
                KeepAlive = true,
                Proxy = proxy
            };
            getRequest.Run(ref cookies);

            var validIndex = getRequest.Response.Contains("Mercedes-Benz WebParts</title>");
            if (validIndex) Config.Instance.AddLogInfo($"Auth result: successful");
            else Config.Instance.AddLogInfo($"Auth result: unknown, something was wrong");
        }
    }
}
