using Logger;

namespace CatalogSupportLibrary
{
    public sealed class Config
    {
        private static volatile Config _instance;
        private static readonly object SyncRoot = new object();
        private Config()
        {
        }

        public static Config Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null) _instance = new Config();
                    }
                }
                return _instance;
            }
        }

        public void AddLogInfo(string log)
        {
            Log.Instance.Info(log, "[CATALOGSUPPORTLIBRARY]");
        }
        public void AddLogError(int errorNumber, string log)
        {
            Log.Instance.Error(errorNumber, log, "[CATALOGSUPPORTLIBRARY]");
        }
    }
}
