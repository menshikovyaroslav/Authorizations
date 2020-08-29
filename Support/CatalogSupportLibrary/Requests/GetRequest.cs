using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Configuration;
using System.Net.Configuration;
using Dom.Extensions;
using Dom.Catalog;

namespace CatalogSupportLibrary.Requests
{
    public class GetRequest
    {
        private HttpWebRequest _request;
        public void Run(ref CookieContainer cookies)
        {
            _request = (HttpWebRequest)WebRequest.Create(Address);

            _request.Headers.Add("DNT", "1");
            _request.Method = "Get";
            _request.Accept = Accept;
            _request.Host = Host;

            if (TurnOffProxy) _request.Proxy = null;
            else _request.Proxy = Proxy;

            if (UseUnsafeHeaderParsing)
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = (SettingsSection)config.GetSection("system.net/settings");
                settings.HttpWebRequest.UseUnsafeHeaderParsing = true;
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("system.net/settings");
            }

            if (TimeOut > 0)
            {
                _request.Timeout = TimeOut;
                _request.ReadWriteTimeout = TimeOut;
            }
            else
            {
                _request.Timeout = 35000;
                _request.ReadWriteTimeout = 35000;
            }

            if (NoCachePolicy == false)
            {
                var noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                _request.CachePolicy = noCachePolicy;
            }

            foreach (KeyValuePair<string, string> keyValuePair in Headers)
            {
                _request.Headers.Add(keyValuePair.Key, keyValuePair.Value);
            }

            if (UserAgent == null) _request.UserAgent = Data.Ie11;
            else _request.UserAgent = UserAgent;


            if (AllowAutoRedirect != null)
                _request.AllowAutoRedirect = (bool)AllowAutoRedirect;

            if (KeepAlive != null)
                _request.KeepAlive = (bool)KeepAlive;

            if (Expect100Continue != null)
                _request.ServicePoint.Expect100Continue = (bool)Expect100Continue;


            if (!Referer.IsEmpty())
                _request.Referer = Referer;

            _request.CookieContainer = cookies;

            try
            {
                HttpWebResponse response = (HttpWebResponse)_request.GetResponse();

                if ((response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Moved || response.StatusCode == HttpStatusCode.Redirect) && response.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase))
                {
                    // if the remote file was found, download oit
                    using (Stream inputStream = response.GetResponseStream())
                    {
                        byte[] buffer = new byte[64000];
                        int bytesRead;

                        bytesRead = inputStream.Read(buffer, 0, buffer.Length);
                        Response = Convert.ToBase64String(buffer, 0, bytesRead);
                    }
                }
                else
                {
                    var stream = response.GetResponseStream();

                    if (stream != null) Response = new StreamReader(stream).ReadToEnd();
                    ResponseHeaders = response.Headers;
                    RequestHeaders = _request.Headers;
                }


                response.Close();
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    Response = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
            }

        }

        Dictionary<string, string> Headers = new Dictionary<string, string>();
        public void AddHeader(string headerName, string headerValue)
        {
            Headers[headerName] = headerValue;
        }
        public bool NoCachePolicy { get; set; }
        public bool AcceptGZipEncoding { get; set; }
        public bool UseUnsafeHeaderParsing { get; set; }
        public string Address { get; set; }
        public string Accept { get; set; }
        public string Referer { get; set; }
        public string Host { get; set; }
        public bool? KeepAlive { get; set; }
        public bool? Expect100Continue { get; set; }
        public string Response { get; private set; }
        public bool? AllowAutoRedirect { get; set; }
        public WebHeaderCollection ResponseHeaders { get; private set; }
        public WebHeaderCollection RequestHeaders { get; private set; }
        public string UserAgent { get; set; }
        public WebProxy Proxy { get; set; }
        public bool TurnOffProxy { get; set; }
        public int TimeOut { get; set; }

    }
}
