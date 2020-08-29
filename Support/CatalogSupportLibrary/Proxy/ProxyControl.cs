using System;
using System.Net;
using System.Runtime.InteropServices;

namespace CatalogSupportLibrary.Proxy
{
    public static class ProxyControl
    {
        #region //wininet Interop

        private enum MyOptions
        {
            InternetPerConnFlags = 1,
            InternetPerConnProxyServer = 2,
            InternetPerConnProxyBypass = 3,
            InternetPerConnAutoconfigUrl = 4,
            InternetPerConnAutodiscoveryFlags = 5,
            InternetOptionRefresh = 37,
            InternetOptionPerConnectionOption = 75,
            InternetOptionSettingsChanged = 39,
            ProxyTypeProxy = 0x00000002,
            ProxyTypeDirect = 0x00000001
        }

        private enum ROptions
        {
            INTERNET_PER_CONN_FLAGS = 1,
            INTERNET_PER_CONN_PROXY_SERVER = 2,
            INTERNET_PER_CONN_PROXY_BYPASS = 3,
            INTERNET_PER_CONN_AUTOCONFIG_URL = 4,
            INTERNET_PER_CONN_AUTODISCOVERY_FLAGS = 5,
            INTERNET_PER_CONN_AUTOCONFIG_SECONDARY_URL = 6,
            INTERNET_PER_CONN_AUTOCONFIG_RELOAD_DELAY_MINS = 7,
            INTERNET_PER_CONN_AUTOCONFIG_LAST_DETECT_TIME = 8,
            INTERNET_PER_CONN_AUTOCONFIG_LAST_DETECT_URL = 9,
            INTERNET_OPTION_REFRESH = 37,
            INTERNET_OPTION_PER_CONNECTION_OPTION = 75,
            INTERNET_OPTION_SETTINGS_CHANGED = 39,
            INTERNET_OPTION_PROXY = 38,
            INTERNET_OPEN_TYPE_PROXY = 3
        }

        [Flags]
        private enum Flags
        {
            PROXY_TYPE_DIRECT = 0x00000001, //Интернет доступ через прямое соединение(без прокси)
            PROXY_TYPE_PROXY = 0x00000002, //Интернет доступ используя прокси
            PROXY_TYPE_AUTO_PROXY_URL = 0x00000004,
            PROXY_TYPE_AUTO_DETECT = 0x00000008
        }

        [Flags]
        private enum ProxyFlags
        {
            AutoProxyFlagUserSet = 0x00000001,
            AutoProxyFlagAlwaysDetect = 0x00000002,
            AutoProxyFlagDetectionRun = 0x00000004,
            AutoProxyFlagMigrated = 0x00000008,
            AutoProxyFlagDontCacheProxyResult = 0x00000010,
            AutoProxyFlagCacheInitRun = 0x00000020,
            AutoProxyFlagDetectionSuspect = 0x00000040
        }

        [StructLayout(LayoutKind.Explicit, Size = 12)]
        private struct INTERNET_PER_CONN_OPTION
        {
            [FieldOffset(0)]
            public int dwOption;
            [FieldOffset(4)]
            public int dwValue;
            [FieldOffset(4)]
            public IntPtr pszValue;
            [FieldOffset(4)]
            public IntPtr ftValue;

            public byte[] GetBytes()
            {
                byte[] b = new byte[12];
                BitConverter.GetBytes(dwOption).CopyTo(b, 0);
                switch (dwOption)
                {
                    case (int)MyOptions.InternetPerConnFlags:
                        BitConverter.GetBytes(dwValue).CopyTo(b, 4);
                        break;
                    case (int)MyOptions.InternetPerConnProxyBypass:
                        BitConverter.GetBytes(pszValue.ToInt32()).CopyTo(b, 4);
                        break;
                    case (int)MyOptions.InternetPerConnProxyServer:
                        BitConverter.GetBytes(pszValue.ToInt32()).CopyTo(b, 4);
                        break;
                }
                return b;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class InternetPerConnOptionList
        {
            public int dwSize;
            public string pszConnection;
            public int dwOptionCount;
            public int dwOptionError;
            public IntPtr pOptions;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct INTERNET_PROXY_INFO
        {
            public int dwAccessType;
            public IntPtr lpszProxy;
            public IntPtr lpszProxyBypass;
        }

        private const int ERROR_INSUFFICIENT_BUFFER = 122;
        private const int INTERNET_OPTION_PROXY = 38;
        private const int INTERNET_OPEN_TYPE_DIRECT = 1;
        private const int INTERNET_OPEN_TYPE_PROXY = 3;

        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetSetOption(IntPtr hInternet, int dwOption,
            InternetPerConnOptionList lpBuffer, int dwBufferLength);

        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer,
            int lpdwBufferLength);

        [DllImport("kernel32.dll")]
        private static extern int GetLastError();

        #endregion

        public static void Set(string proxy)
        {
            try
            {
                const int INTERNET_OPTION_PROXY = 38;
                const int INTERNET_OPEN_TYPE_PROXY = 3;

                Struct_INTERNET_PROXY_INFO struct_IPI;

                // Filling in structure
                struct_IPI.dwAccessType = INTERNET_OPEN_TYPE_PROXY;
                struct_IPI.proxy = Marshal.StringToHGlobalAnsi(proxy);
                struct_IPI.proxyBypass = Marshal.StringToHGlobalAnsi("local");

                // Allocating memory
                IntPtr intptrStruct = Marshal.AllocCoTaskMem(Marshal.SizeOf(struct_IPI));

                // Converting structure to IntPtr
                Marshal.StructureToPtr(struct_IPI, intptrStruct, true);

                bool iReturn = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_PROXY, intptrStruct, Marshal.SizeOf(struct_IPI));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public struct Struct_INTERNET_PROXY_INFO
        {
            public int dwAccessType;
            public IntPtr proxy;
            public IntPtr proxyBypass;
        };
        private static bool ProxyEnable(WebProxy wProxy)
        {
            try
            {
                bool bReturn = true;
                string sPrx = wProxy.Address.DnsSafeHost + ":" + wProxy.Address.Port;
                InternetPerConnOptionList list = new InternetPerConnOptionList();
                int dwBufSize = Marshal.SizeOf(list);
                INTERNET_PER_CONN_OPTION[] opts = new INTERNET_PER_CONN_OPTION[3];
                int opt_size = Marshal.SizeOf(opts[0]);
                list.dwSize = dwBufSize;
                list.pszConnection = String.Empty;
                list.dwOptionCount = 3;
                //set flags
                opts[0].dwOption = (int)MyOptions.InternetPerConnFlags;
                //opts[0].dwValue = (int)(Options.PROXY_TYPE_DIRECT | Options.PROXY_TYPE_PROXY);
                //opts[0].dwValue = (int)(MyOptions.PROXY_TYPE_DIRECT | MyOptions.PROXY_TYPE_PROXY);
                opts[0].dwValue = (int)(Flags.PROXY_TYPE_PROXY);
                //set proxyname
                opts[1].dwOption = (int)MyOptions.InternetPerConnProxyServer;
                //opts[1].pszValue = Marshal.StringToHGlobalAnsi("http://" + sPrx);
                opts[1].pszValue = Marshal.StringToHGlobalAnsi(sPrx);
                //opts[1].pszValue = Marshal.StringToCoTaskMemAnsi("http=http://" + sPrx + "; ftp=ftp://" + sPrx + "; https=https://" + sPrx + "; gopher=gopher://" + sPrx + "; socks=socks://" + sPrx);
                //set override
                opts[2].dwOption = (int)MyOptions.InternetPerConnProxyBypass;
                opts[2].pszValue = Marshal.StringToHGlobalAnsi("localhost");
                //opts[2].pszValue = Marshal.StringToCoTaskMemAnsi("<local>localhost; rado.ra-host.com");
                byte[] b = new byte[3 * opt_size];
                opts[0].GetBytes().CopyTo(b, 0);
                opts[1].GetBytes().CopyTo(b, opt_size);
                opts[2].GetBytes().CopyTo(b, 2 * opt_size);
                IntPtr ptr = Marshal.AllocCoTaskMem(3 * opt_size);
                Marshal.Copy(b, 0, ptr, 3 * opt_size);
                list.pOptions = ptr;
                //set the options on the connection
                bReturn = InternetSetOption(IntPtr.Zero, (int)MyOptions.InternetOptionPerConnectionOption, list,
                    dwBufSize);
                if (!bReturn)
                {
                    Console.WriteLine(GetLastError());
                }
                //Flush the current IE proxy setting
                bReturn = InternetSetOption(IntPtr.Zero, (int)MyOptions.InternetOptionRefresh, IntPtr.Zero, 0);
                if (!bReturn)
                {
                    Console.WriteLine(GetLastError());
                }
                Marshal.FreeHGlobal(opts[1].pszValue);
                Marshal.FreeHGlobal(opts[2].pszValue);
                Marshal.FreeCoTaskMem(ptr);
                return bReturn;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }


}
