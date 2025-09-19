namespace LoadBalancer
{
    public class ServerSelector : IServerSelector
    {
        public Server? SelectRandom(IReadOnlyList<Server> servers)
        {
            var ups = new List<Server>(servers.Count);
            for (int i = 0; i < servers.Count; i++)
            {
                if (servers[i].up) ups.Add(servers[i]);
            }

            if (ups.Count == 0)
            {
                Console.WriteLine("No available servers");
                return null;
            }

            int index = Random.Shared.Next(ups.Count);
            return ups[index];
        }
    }
}
