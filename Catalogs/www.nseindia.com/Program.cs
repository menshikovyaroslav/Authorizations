using CatalogSupportLibrary.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace www.nseindia.com
{
    class Program
    {
        static void Main(string[] args)
        {
            // We keep cookies here
            var cookies = new CookieContainer();

            // Any proxy, for example Fiddler
            var proxy = new WebProxy("127.0.0.1:8888");

            var uri = new Uri("https://www.nseindia.com");
            cookies.Add(new Cookie("bm_mi", "A65ADC53049AD0E5266F175C12C57F69~iR4bU8FbIYYgbGqq+TVwNqpg/eqh/1Gvqtt/9mrlRETWJ9uXAdeAZ4MP+wKKoS3AorSZo8av0PMQcmrS57tMZOrvTV0FTlSiD2sKGR7Z+tQ1SaMSfs/EC9Q7jAjpU3o23kNAwd0x51GFKlYBwMfaXGN8fZHS2Pd7qSOhyyylyDWnQYZ5JHGr0eeL/X42QSMBrXK4GFZ5MMIjgPrkBQ6+iVM04QseCmtXiKKFn8xLWJauCtr2w6lA/lkYXXnoOVQxIoD2t/1ZysSHDFvLmZfKcA==") { Domain = uri.Host });
            cookies.Add(new Cookie("ak_bmsc", "4C7337DAF3ACAD63A712D6565E3EA44D17035A1C13110000AA72B55FDC118F24~plgHAREOwPvQrizD4S5TvX1v07NvxkVghUbqVPJbH2+aE836TR+qTZ44O51gA+yAG/J9uzhpYHEHAQpxIWgRQFwlSbCDhE6AheKgcnA8T+mEGAPGI/WNXXMtIKwNrV4Z0Td5Z1yaEUwXgq+dV/Zao4YTiHUuVeCDuovsBA2x6NxVvG7OktOsqfWbQxSOTijcwMPdC8oU8ppXVzztWafBXPLefLYQA87Wnb4m3w3usYHaAaufinMbJtO6plvPD0hGs7") { Domain = uri.Host });
            cookies.Add(new Cookie("bm_sv", "C2667D2FAD194DCE07426CC0F93717EB~wIr7+qVfuvMzZe/mhfyCvplhMYsEsQReTjIjFC8nA6P3gbQwGVTvUJONN5z854Nwm/taYcKsK9MrZqmkvbOJWBYyvemQNr29dG1iZUJhCQwI4j8lxZhFgvQCI3SGAcZ9p9iQMhnhakvpyaU+nTxZ4BCroBWmrE0mAtiGDCvg9OY=") { Domain = uri.Host });
            cookies.Add(new Cookie("nseappid", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJhcGkubnNlIiwiYXVkIjoiYXBpLm5zZSIsImlhdCI6MTYwNTcyNjkwNSwiZXhwIjoxNjA1NzMwNTA1fQ.V8iLKGmszkBrgGm-fWfYr6UATE8UeU_Rl3n6BBE6Ozg") { Domain = uri.Host });
            cookies.Add(new Cookie("nsit", "oeUI_ijv8fFrIo_3ek_iFrof") { Domain = uri.Host });

            var getRequest = new GetRequest()
            {
                Address = "https://www.nseindia.com/api/option-chain-equities?symbol=APOLLOHOSP",
                Accept = "text/html, application/xhtml+xml, */*",
                Host = "www.nseindia.com",
                Proxy = proxy,
                KeepAlive = true
            };
            getRequest.Run(ref cookies);
        }
    }
}
