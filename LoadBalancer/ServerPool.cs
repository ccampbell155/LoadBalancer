namespace LoadBalancer
{
    public class ServerPool
    {
        private readonly List<Server> _servers = new();
        public ServerPool(IEnumerable<Server> servers)
        {
            _servers.AddRange(servers);
        }
        public IReadOnlyList<Server> Servers
        {
            get
            {
                return _servers;
            }
        }
    }
}
