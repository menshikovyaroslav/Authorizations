using Dom.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Text;

namespace CatalogSupportLibrary.Requests
{
    public class PostRequest
    {
        private HttpWebRequest _request;
        public void Run(ref CookieContainer cookies)
        {
            _request = (HttpWebRequest)WebRequest.Create(Address);
            _request.Method = "POST";
            _request.Host = Host;
            _request.Headers.Add("DNT", "1");
            _request.Proxy = Proxy;

            _request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            if (TimeOut > 0)
            {
                _request.Timeout = TimeOut;
                _request.ReadWriteTimeout = TimeOut;
            }
            else
            {
                _request.Timeout = 90000;
                _request.ReadWriteTimeout = 90000;
            }

      //      if (NoCachePolicy == false)
      //      {
                var noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                _request.CachePolicy = noCachePolicy;
     //       }

            if (Expect100Continue == true) _request.ServicePoint.Expect100Continue = true;
            else _request.ServicePoint.Expect100Continue = false;

            _request.ContentType = ContentType;
            _request.Accept = Accept;
            _request.Referer = Referer;


            _request.KeepAlive = KeepAlive;


       //     _request.KeepAlive = true;

            foreach (KeyValuePair<string, string> keyValuePair in Headers)
            {
                _request.Headers.Add(keyValuePair.Key, keyValuePair.Value);
            }

            _request.CookieContainer = cookies;

            if (UserAgent == null) _request.UserAgent = Dom.Catalog.Data.Ie11;
            else _request.UserAgent = UserAgent;

            if (AllowAutoRedirect != null)
                _request.AllowAutoRedirect = (bool)AllowAutoRedirect;


            byte[] sentData;
            if (ByteData != null) sentData = ByteData;
            else sentData = Encoding.UTF8.GetBytes(Data);


            _request.ContentLength = sentData.Length;
            Stream sendStream = _request.GetRequestStream();
            sendStream.Write(sentData, 0, sentData.Length);
            sendStream.Close();
            WebResponse response = _request.GetResponse();

            ResponseHeaders = response.Headers;
            RequestHeaders = _request.Headers;

            Stream stream = response.GetResponseStream();
            if (stream != null)
            {
                if (ResponseEncoding.IsEmpty()) Response = new StreamReader(stream).ReadToEnd();
                else Response = new StreamReader(stream, Encoding.GetEncoding(ResponseEncoding)).ReadToEnd();
            }
            response.Close();
        }

        Dictionary<string, string> Headers = new Dictionary<string, string>();
        public void AddHeader(string headerName, string headerValue)
        {
            Headers[headerName] = headerValue;
        }
        public bool NoCachePolicy { get; set; }
        public string Response { get; set; }
        public string ResponseEncoding { get; set; }
        public string Data { get; set; }
        public byte[] ByteData { get; set; }
        public string Address { get; set; }
        public string Accept { get; set; }
        public string Host { get; set; }
        public string ContentType { get; set; }
        public string Referer { get; set; }
        public bool KeepAlive { get; set; }
        public bool? Expect100Continue { get; set; }
        public string UserAgent { get; set; }
        public bool? AllowAutoRedirect { get; set; }
        public WebHeaderCollection ResponseHeaders { get; private set; }
        public WebHeaderCollection RequestHeaders { get; private set; }
        public WebProxy Proxy { get; set; }
        public int TimeOut { get; set; }
    }
}
