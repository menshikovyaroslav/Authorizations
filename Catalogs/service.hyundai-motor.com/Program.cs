using CatalogSupportLibrary;
using CatalogSupportLibrary.Requests;
using Dom.Extensions;
using System.Net;

namespace service.hyundai_motor.com
{
    class Program
    {
        static void Main(string[] args)
        {
            // Each hyundai account have two parameters : User, Password
            var user = "user";              // !!! rewrite value from your account !!!
            var password = "password";      // !!! rewrite value from your account !!!

            // We keep cookies here
            var cookies = new CookieContainer();

            // Any proxy, for example Fiddler
            var proxy = new WebProxy("127.0.0.1:8888");

            var getRequest = new GetRequest()
            {
                Address = "https://service.hyundai-motor.com/euro5/login.tiles",
                Accept = "text/html, application/xhtml+xml, */*",
                Host = "service.hyundai-motor.com",
                KeepAlive = true,
                Proxy = proxy
            };
            getRequest.Run(ref cookies);

            // this is a kia feature to send login only in upcase
            //login and password we need to encode to base64
            var userBase64 = user.ToUpper().ToBase64();
            var passBase64 = password.ToBase64();
            var data = "{\"userid\":\"" + userBase64 + "\",\"passwd\":\"" + passBase64 + "\",\"loginType\":\"RVVSTzU=\"}";

            // Auth request to kia portal
            var postRequest = new PostRequest()
            {
                Data = data,
                Address = $"https://service.hyundai-motor.com/common/chinalogin.action?",
                Accept = "application/json, text/javascript, */*; q=0.01",
                Host = "service.hyundai-motor.com",
                ContentType = "application/x-www-form-urlencoded",
                Referer = "https://service.hyundai-motor.com/euro5/login.tiles",
                KeepAlive = true,
                Proxy = proxy
            };
            postRequest.Run(ref cookies);

            if (postRequest.Response.Contains("result_code\":\"99"))
            {
                // Need to change password
                Config.Instance.AddLogInfo($"Auth isn't good");
                return;
            }

            getRequest = new GetRequest()
            {
                Address = "http://service.hyundai-motor.com/manualV2/euro5/cnts/view/PARTS?euro5=1",
                Accept = "application/json, text/javascript, */*; q=0.01",
                Host = "service.hyundai-motor.com",
                KeepAlive = true,
                Referer = "https://service.hyundai-motor.com/euro5/index.tiles",
                Proxy = proxy
            };
            getRequest.Run(ref cookies);

            #region sad part of finding data for post request

            var group_s = getRequest.Response.IndexOf("\"group\"") + 9;
            var group_e = getRequest.Response.IndexOf("\"", group_s);
            var group = getRequest.Response.Substring(group_s, group_e - group_s);

            var id_s = getRequest.Response.IndexOf("\"wpcId\"") + 9;
            var id_e = getRequest.Response.IndexOf("\"", id_s);
            var id = getRequest.Response.Substring(id_s, id_e - id_s);

            var key_s = getRequest.Response.IndexOf("\"wpcKey\"") + 10;
            var key_e = getRequest.Response.IndexOf("\"", key_s);
            var key = getRequest.Response.Substring(key_s, key_e - key_s);

            var natcd_s = getRequest.Response.IndexOf("\"natCd\"") + 9;
            var natcd_e = getRequest.Response.IndexOf("\"", natcd_s);
            var natcd = getRequest.Response.Substring(natcd_s, natcd_e - natcd_s);

            var uid_s = getRequest.Response.IndexOf("\"userId\"") + 10;
            var uid_e = getRequest.Response.IndexOf("\"", uid_s + 1);
            var userId = getRequest.Response.Substring(uid_s, uid_e - uid_s);

            var reg_s = getRequest.Response.IndexOf("\"reg\"") + 7;
            var reg_e = getRequest.Response.IndexOf("\"", reg_s + 1);
            var reg = getRequest.Response.Substring(reg_s, reg_e - reg_s);

            #endregion

            data = $"cmd=Login&group={group}&id={id}&key={key}&natCd={natcd}&userid={userId}&reg={reg}";

            // Auth request to mobis portal
            postRequest = new PostRequest()
            {
                Data = data,
                Address = $"https://wpc.mobis.co.kr/SsoLogin",
                Accept = "text/html, application/xhtml+xml, */*",
                Host = "wpc.mobis.co.kr",
                ContentType = "application/x-www-form-urlencoded",
                Referer = "https://service.hyundai-motor.com/euro5/index.tiles",
                KeepAlive = true,
                TimeOut = 60000,
                Proxy = proxy
            };
            postRequest.Run(ref cookies);

            // we need cookies only from mobis.co
            var catalogCookies = cookies.GetCookieCollection().CatalogCookies("mobis.co");

            // let check our auth
            getRequest = new GetRequest()
            {
                Address = "http://wpc.mobis.co.kr/Index/Index.jsp",
                Accept = "text/html, application/xhtml+xml, */*",
                Host = "wpc.mobis.co.kr",
                KeepAlive = true,
                Proxy = proxy
            };
            getRequest.Run(ref cookies);

            var validIndex = getRequest.Response.IndexOf("<title>TEPS - WPC</title>");
            if (validIndex >= 0) Config.Instance.AddLogInfo($"Auth result: successful");
            else
            {
                Config.Instance.AddLogInfo($"Auth result: unknown, something was wrong");
                return;
            }

            // writing cookies in log
            foreach (var cookie in catalogCookies)
            {
                Config.Instance.AddLogInfo($"cookie: {cookie}");
            }
        }
    }
}
