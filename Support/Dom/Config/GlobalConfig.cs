using Logger;

namespace Dom.Config
{
    public class GlobalConfig
    {
        private static volatile GlobalConfig _instance;
        private static readonly object SyncRoot = new object();

        public GlobalConfig()
        {
        }
        public void AddLogInfo(string log)
        {
            Log.Instance.Info(log, ProgramUniqueName);
        }
        public void AddLogError(int errorNumber, string log)
        {
            Log.Instance.Error(errorNumber, log, ProgramUniqueName);
        }
        public static GlobalConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null) _instance = new GlobalConfig();
                    }
                }
                return _instance;
            }
        }
        public string ProgramUniqueName { get; set; }
    }
}
