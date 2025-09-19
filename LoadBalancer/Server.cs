namespace LoadBalancer
{
    public class Server
    {
        public string host;
        public int port;
        public bool up = false;
        public Server(string h, int p) { host = h; port = p; }
    }
}
