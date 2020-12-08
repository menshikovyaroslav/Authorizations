using CatalogSupportLibrary;
using CatalogSupportLibrary.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace blablacar.ru
{
    class Program
    {
        static void Main(string[] args)
        {
            // Each blablacar account has two parameters : User, Password
            var user = "user";              // !!! rewrite value from your account !!!
            var password = "password";      // !!! rewrite value from your account !!!

            // We keep cookies here
            var cookies = new CookieContainer();

            // Any proxy, for example Fiddler
            var proxy = new WebProxy("127.0.0.1:8888");

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var getRequest = new GetRequest()
            {
                Address = "https://www.blablacar.ru/",
                Accept = "text/html, application/xhtml+xml, image/jxr, */*",
                Host = "www.blablacar.ru",
                KeepAlive = true,
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko",
                Proxy = proxy
            };
            getRequest.Run(ref cookies);

            // We should to find visitor value
            var visitorStart = getRequest.Response.IndexOf("visitorId") + 12;
            var visitorEnd = getRequest.Response.IndexOf("\"", visitorStart);
            var visitorId = getRequest.Response.Substring(visitorStart, visitorEnd - visitorStart);

            // Random value
            var xCorrelationId = Guid.NewGuid();

            // Auth request
            var data = $"{{\"login\":\"{user}\",\"password\":\"{password}\",\"rememberMe\":true,\"grant_type\":\"password\"}}";
            var postRequest = new PostRequest()
            {
                Data = data,
                Address = $"https://auth.blablacar.ru/secure-token",
                Accept = "application/json",
                Host = "auth.blablacar.ru",
                ContentType = "application/json",
                Referer = "https://www.blablacar.ru/login/email",
                KeepAlive = true,
                Proxy = proxy
            };
            postRequest.AddHeader("x-client", "SPA|1.0.0");
            postRequest.AddHeader("x-correlation-id", xCorrelationId.ToString());
            postRequest.AddHeader("x-currency", "RUB");
            postRequest.AddHeader("x-forwarded-proto", "https");
            postRequest.AddHeader("x-locale", "ru_RU");
            postRequest.AddHeader("x-visitor-id", visitorId);
            postRequest.AddHeader("Origin", "https://www.blablacar.ru");
            postRequest.Run(ref cookies);

            // Finding this cookie value from all the headers
            var bearerCookieValue = string.Empty;
            var headers = postRequest.ResponseHeaders;
            for (int i = 0; i < headers.Count; ++i)
            {
                string header = headers.GetKey(i);

                if (header == "Set-Cookie")
                {
                    var headerValue = headers.GetValues(i)[0];
                    headerValue = WebUtility.UrlDecode(headerValue);

                    var accessTokenStart = postRequest.Response.IndexOf("access_token") + 15;
                    var accessTokenEnd = postRequest.Response.IndexOf("\"", accessTokenStart);
                    bearerCookieValue = postRequest.Response.Substring(accessTokenStart, accessTokenEnd - accessTokenStart);
                }
            }

            // For example we want to know all the trips from this account
            getRequest = new GetRequest()
            {
                Address = "https://edge.blablacar.ru/bookings-and-tripoffers?active=false",
                Accept = "application/json",
                Host = "edge.blablacar.ru",
                KeepAlive = true,
                ContentType = "application/json",
                Referer = "https://www.blablacar.ru/rides/history",
                Proxy = proxy
            };
            getRequest.AddHeader("x-blablacar-accept-endpoint-version", "2");
            getRequest.AddHeader("x-client", "SPA|1.0.0");
            getRequest.AddHeader("x-correlation-id", xCorrelationId.ToString());
            getRequest.AddHeader("x-currency", "RUB");
            getRequest.AddHeader("x-forwarded-proto", "https");
            getRequest.AddHeader("x-locale", "ru_RU");
            getRequest.AddHeader("x-visitor-id", visitorId);
            getRequest.AddHeader("authorization", $"Bearer {bearerCookieValue}");
            getRequest.AddHeader("Origin", "https://www.blablacar.ru");
            getRequest.Run(ref cookies);

            // Write to log the json response with all the account trips
            Config.Instance.AddLogInfo(getRequest.Response);


            var date = "2020-12-15";
            var searchUid = Guid.NewGuid();

            // For example we want to know all the trips from one point to another on any date
            getRequest = new GetRequest()
            {
                Address = $"https://edge.blablacar.ru/trip/search?from_coordinates=55.755826%2C37.617299&from_country=RU&to_coordinates=59.931058%2C30.360909&to_country=RU&departure_date={date}&min_departure_time=00%3A00%3A00&requested_seats=1&passenger_gender=UNKNOWN&search_uuid={searchUid}",
                Accept = "application/json",
                Host = "edge.blablacar.ru",
                KeepAlive = true,
                ContentType = "application/json",
                Referer = "https://www.blablacar.ru/",
                Proxy = proxy
            };
            getRequest.AddHeader("x-blablacar-accept-endpoint-version", "2");
            getRequest.AddHeader("x-client", "SPA|1.0.0");
            getRequest.AddHeader("x-correlation-id", xCorrelationId.ToString());
            getRequest.AddHeader("x-currency", "RUB");
            getRequest.AddHeader("x-forwarded-proto", "https");
            getRequest.AddHeader("x-locale", "ru_RU");
            getRequest.AddHeader("x-visitor-id", visitorId);
            getRequest.AddHeader("authorization", $"Bearer {bearerCookieValue}");
            getRequest.AddHeader("Origin", "https://www.blablacar.ru");
            getRequest.Run(ref cookies);

        }
    }
}
