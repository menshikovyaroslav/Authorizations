namespace Dom
{
    public class Proxy
    {
        public string Ip { get; set; }
        public int Port { get; set; }
        public Proxy(string ip, int port)
        {
            Ip = ip;
            Port = port;
        }

        public override string ToString()
        {
            return $"{Ip}:{Port}";
        }
    }
}
