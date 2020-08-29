using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using Dom.Extensions;
using Dom.Catalog;

namespace CatalogSupportLibrary
{
    public static class Methods
    {
        private const int InternetOptionEndBrowserSession = 42;

        // Для изменения UserAgent
        [DllImport("urlmon.dll", CharSet = CharSet.Ansi)]
        private static extern int UrlMkSetSessionOption(int dwOption, string pBuffer, int dwBufferLength, int dwReserved);

        // Для импорта куки
        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool InternetSetCookie(string lpszUrlName, string lpszCookieName, string lpszCookieData);
        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);
        [DllImport("wininet.dll", SetLastError = true)]
        public static extern bool InternetGetCookieEx(
        string url,
        string cookieName,
        StringBuilder cookieData,
        ref int size,
        Int32 dwFlags,
        IntPtr lpReserved);

        private const Int32 InternetCookieHttponly = 0x2000;

        public static void SetAnotherUserAgent(string ua)
        {
            const int urlmonOptionUseragent = 0x10000001;
            const int urlmonOptionUseragentRefresh = 0x10000002;
            var UserAgent = ua;
            UrlMkSetSessionOption(urlmonOptionUseragentRefresh, null, 0, 0);
            UrlMkSetSessionOption(urlmonOptionUseragent, UserAgent, UserAgent.Length, 0);
        }

        public static CookieContainer GetUriCookieContainer(Uri uri)
        {
            CookieContainer cookies = null;
            // Determine the size of the cookie
            int datasize = 8192 * 16;
            StringBuilder cookieData = new StringBuilder(datasize);
            if (!InternetGetCookieEx(uri.ToString(), null, cookieData, ref datasize, InternetCookieHttponly, IntPtr.Zero))
            {
                if (datasize < 0)
                    return null;
                // Allocate stringbuilder large enough to hold the cookie
                cookieData = new StringBuilder(datasize);
                if (!InternetGetCookieEx(
                    uri.ToString(),
                    null, cookieData,
                    ref datasize,
                    InternetCookieHttponly,
                    IntPtr.Zero))
                    return null;
            }
            cookies = new CookieContainer();
            if (cookieData.Length > 0)
            {
                cookies.SetCookies(uri, cookieData.ToString().Replace(';', ','));
            }
            return cookies;
        }

        public static List<string> GetCookiesFromUri(Uri uri)
        {
            CookieContainer cookies = null;
            var result = new List<string>();
            // Determine the size of the cookie
            int datasize = 8192 * 16;
            StringBuilder cookieData = new StringBuilder(datasize);
            if (!InternetGetCookieEx(uri.ToString(), null, cookieData, ref datasize, InternetCookieHttponly, IntPtr.Zero))
            {
                if (datasize < 0)
                    return null;
                // Allocate stringbuilder large enough to hold the cookie
                cookieData = new StringBuilder(datasize);
                if (!InternetGetCookieEx(
                    uri.ToString(),
                    null, cookieData,
                    ref datasize,
                    InternetCookieHttponly,
                    IntPtr.Zero))
                    return null;
            }
            cookies = new CookieContainer();
            if (cookieData.Length > 0)
            {
                cookies.SetCookies(uri, cookieData.ToString().Replace(';', ','));
            }

            foreach (Cookie cookie in cookies.GetCookieCollection())
            {
                result.Add($"{cookie.Name}={cookie.Value}");
            }

            return result;
        }

        private static void DisableCachingOfSslPages()
        {
            // По умолчанию выключаем отключение этой настройки

            RegistryKey hk = Registry.CurrentUser;
            RegistryKey ie = hk.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);

            ie.SetValue("DisableCachingOfSSLPages", 0, RegistryValueKind.DWord);
            
            ie?.Close();
            hk?.Close();
        }

        public static void DisablePopupBlocker()
        {
            RegistryKey hk = Registry.CurrentUser;
            RegistryKey ie = hk.OpenSubKey("Software\\Microsoft\\Internet Explorer\\New Windows", true);

            ie.SetValue("PopupMgr", 0);

            ie?.Close();
            hk?.Close();
        }


        private static void Disable1609()
        {
            // По умолчанию выключаем отключение этой настройки

            RegistryKey hk = Registry.CurrentUser;
            RegistryKey ie = hk.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Zones\\3", true);

            ie.SetValue("1609", 0, RegistryValueKind.DWord);

            ie?.Close();
            hk?.Close();
        }

        public static string GetSecureProtocols()
        {
            RegistryKey hk = Registry.CurrentUser;
            RegistryKey ie = hk.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings");

            var x = ie.GetValue("SecureProtocols").ToString();

            ie?.Close();
            hk?.Close();

            return x;
        }

        public static void SetSecureProtocols()
        {
            RegistryKey hk = Registry.CurrentUser;
            RegistryKey ie = hk.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);

            ie.SetValue("SecureProtocols", 800, RegistryValueKind.DWord);

            ie?.Close();
            hk?.Close();
        }

        public static void EnterRegistryKeys()
        {
            try
            {
                Disable1609();
                DisableCachingOfSslPages();
                WarnOfCertReceivingOff();
                WarnonZoneCrossingOff();
            //    SetSecureProtocols();
            }
            catch (Exception e)
            {
                Config.Instance.AddLogInfo(e.Message);
            }
        }

        public static void TurnOffCacheForIe(bool turnOffCache = true)
        {
            RegistryKey hk = Registry.CurrentUser;
            RegistryKey ie = hk.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);

            if (turnOffCache) ie.SetValue("SyncMode5", "0");
            else ie.SetValue("SyncMode5", "4");

            ie?.Close();
            hk?.Close();
        }

        public static void ClearIeCache(int level)
        {
            var t = new Thread(() =>
            {
                ProcessStartInfo p = new ProcessStartInfo("RunDll32.exe");
                p.Arguments = $"InetCpl.cpl,ClearMyTracksByProcess {level}";
                Process.Start(p);
            });
            t.IsBackground = true;
            t.Start();
        }

        public static void SetChrome52UserAgent()
        {
            const int urlmonOptionUseragent = 0x10000001;
            const int urlmonOptionUseragentRefresh = 0x10000002;
            var UserAgent = Data.Awesomium;
            UrlMkSetSessionOption(urlmonOptionUseragentRefresh, null, 0, 0);
            UrlMkSetSessionOption(urlmonOptionUseragent, UserAgent, UserAgent.Length, 0);
        }
        public static void SetIeUserAgent()
        {
            const int urlmonOptionUseragent = 0x10000001;
            const int urlmonOptionUseragentRefresh = 0x10000002;
            var UserAgent = Data.Ie11;
            UrlMkSetSessionOption(urlmonOptionUseragentRefresh, null, 0, 0);
            UrlMkSetSessionOption(urlmonOptionUseragent, UserAgent, UserAgent.Length, 0);
        }
        public static void SetIe9UserAgent()
        {
            const int urlmonOptionUseragent = 0x10000001;
            const int urlmonOptionUseragentRefresh = 0x10000002;
            var UserAgent = Data.Ie9;
            UrlMkSetSessionOption(urlmonOptionUseragentRefresh, null, 0, 0);
            UrlMkSetSessionOption(urlmonOptionUseragent, UserAgent, UserAgent.Length, 0);
        }
        public static void SetIe10UserAgent()
        {
            const int urlmonOptionUseragent = 0x10000001;
            const int urlmonOptionUseragentRefresh = 0x10000002;
            var UserAgent = Data.Ie10;
            UrlMkSetSessionOption(urlmonOptionUseragentRefresh, null, 0, 0);
            UrlMkSetSessionOption(urlmonOptionUseragent, UserAgent, UserAgent.Length, 0);
        }
        public static void SetIe7UserAgent()
        {
            const int urlmonOptionUseragent = 0x10000001;
            const int urlmonOptionUseragentRefresh = 0x10000002;
            var UserAgent = Data.Ie7;
            UrlMkSetSessionOption(urlmonOptionUseragentRefresh, null, 0, 0);
            UrlMkSetSessionOption(urlmonOptionUseragent, UserAgent, UserAgent.Length, 0);
        }
        public static void SetIe8UserAgent()
        {
            const int urlmonOptionUseragent = 0x10000001;
            const int urlmonOptionUseragentRefresh = 0x10000002;
            var UserAgent = Data.Ie8;
            UrlMkSetSessionOption(urlmonOptionUseragentRefresh, null, 0, 0);
            UrlMkSetSessionOption(urlmonOptionUseragent, UserAgent, UserAgent.Length, 0);
        }
        public static void SetIe6UserAgent()
        {
            const int urlmonOptionUseragent = 0x10000001;
            const int urlmonOptionUseragentRefresh = 0x10000002;
            var UserAgent = Data.Ie6;
            UrlMkSetSessionOption(urlmonOptionUseragentRefresh, null, 0, 0);
            UrlMkSetSessionOption(urlmonOptionUseragent, UserAgent, UserAgent.Length, 0);
        }

        public static bool IsIe11Emulation()
        {
            try
            {
                var key = Application.ExecutablePath.Split('\\').Reverse().ToList()[0];
                RegistryKey hklm = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Internet Explorer\\MAIN\\FeatureControl\\FEATURE_BROWSER_EMULATION", false);
                var value = hklm.GetValue(key, 0).ToString();
                hklm.Close();

                if (value == "11000" || value == "11001") return true;
            }
            catch (Exception exception)
            {
                return false;
            }
            try
            {
                var key = Application.ExecutablePath.Split('\\').Reverse().ToList()[0];
                RegistryKey hkcu = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Internet Explorer\\MAIN\\FeatureControl\\FEATURE_BROWSER_EMULATION", false);
                var value = hkcu.GetValue(key, 0).ToString();
                hkcu.Close();

                if (value == "11000" || value == "11001") return true;
            }
            catch (Exception exception)
            {
                return false;
            }
            return false;
        }

        public static void ImportCookie(string host, Cookie cookie)
        {
            InternetSetCookie(host, null, cookie.ToString());
        }
        public static void ImportCookie(string host, string key, string value)
        {
            var cookie = new Cookie(key, value);
            InternetSetCookie(host, null, cookie.ToString());
        }
        public static void ImportCookie(string host, string key, string value, ref CookieContainer cookies)
        {
            var cookie = new Cookie(key, value);
            InternetSetCookie(host, null, cookie.ToString());
            cookies.Add(new Uri(host), cookie);
        }

        public static string GetValue(string path, string name)
        {
            return (string)Registry.GetValue(path, name, null);
        }
        public static string GetValue(string path, string subname, string name)
        {
            return (string)Registry.GetValue(Path.Combine(path, subname), name, null);
        }
        public static List<string> GetMultiRowValue(string path, string name)
        {
            return ((string[])(Registry.GetValue(path, name, new string[0]))).ToList();
        }

        private static void WarnOfCertReceivingOff()
        {
            try
            {
                RegistryKey hkcu = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
                hkcu.SetValue("CertificateRevocation", 0, RegistryValueKind.DWord);
                hkcu.Close();
            }
            catch (Exception exception)
            {
                Config.Instance.AddLogError(75, exception.Message);
            }
        }

        private static void WarnonZoneCrossingOff()
        {
            try
            {
                RegistryKey hkcu = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
                hkcu.SetValue("WarnonZoneCrossing", 0, RegistryValueKind.DWord);
                hkcu.Close();
            }
            catch (Exception exception)
            {
                Config.Instance.AddLogError(76, exception.Message);
            }
        }

        public static void SetBrowserEmulation(int version = 11001)
        {
            try
            {
                string program = Application.ExecutablePath.Split('\\').Reverse().ToList()[0];
                var key32 = "SOFTWARE\\Wow6432Node\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_BROWSER_EMULATION";
                var key64 = "SOFTWARE\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_BROWSER_EMULATION";

                try
                {
                    // AppDomain.CurrentDomain.FriendlyName
                    RegistryKey hkcu32 = Registry.CurrentUser.CreateSubKey(key32);

                    if (version == 0) hkcu32.DeleteValue(program, false);
                    else
                    {
                        hkcu32.SetValue(program, version, RegistryValueKind.DWord);
                    }
       
                    hkcu32.Close();
                }
                catch (Exception)
                {
                }

                try
                {
                    RegistryKey hkcu64 = Registry.CurrentUser.CreateSubKey(key64);

                    if (version == 0) hkcu64.DeleteValue(program, false);
                    else
                    {
                        hkcu64.SetValue(program, version, RegistryValueKind.DWord);
                    }
       
                    hkcu64.Close();
                }
                catch (Exception)
                {
                }     
            }
            catch (Exception exception)
            {
                Config.Instance.AddLogError(71, exception.Message);
            }
        }

        public static void IeToolBarOff()
        {
            try
            {
                RegistryKey ie = Registry.CurrentUser.OpenSubKey("Software\\Policies\\Microsoft\\Internet Explorer", true);

                RegistryKey toolbars = ie.OpenSubKey("Toolbars", true);
                if (toolbars == null)
                {
                    ie.CreateSubKey("Toolbars");
                    toolbars = ie.OpenSubKey("Toolbars", true);
                }

                RegistryKey restrictions = toolbars.OpenSubKey("Restrictions", true);
                if (restrictions == null)
                {
                    toolbars.CreateSubKey("Restrictions");
                    restrictions = toolbars.OpenSubKey("Restrictions", true);
                }

                restrictions.SetValue("NoNavBar", 1, RegistryValueKind.DWord);

                restrictions.Close();
                toolbars.Close();
                ie.Close();
            }
            catch (Exception exception)
            {
                Config.Instance.AddLogError(70, exception.Message);
            }
        }

        public static void IeCtrlUOff()
        {
            try
            {
                RegistryKey ie = Registry.CurrentUser.OpenSubKey("Software\\Policies\\Microsoft\\Internet Explorer", true);

                RegistryKey restrictions = ie.OpenSubKey("Restrictions", true);
                if (restrictions == null)
                {
                    ie.CreateSubKey("Restrictions");
                    restrictions = ie.OpenSubKey("Restrictions", true);
                }

                restrictions.SetValue("NoViewSource", 1, RegistryValueKind.DWord);

                restrictions.Close();
                ie.Close();
            }
            catch (Exception exception)
            {
                Config.Instance.AddLogError(79, exception.Message);
            }
        }

    }
}
