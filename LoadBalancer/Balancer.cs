using System.Net;
using System.Net.Sockets;

class Server
{
    public string host;
    public int port;
    public bool up = false;
    public Server(string h, int p) { host = h; port = p; }
}

class ServerPool
{
    public List<Server> list = new();
    public ServerPool(IEnumerable<Server> servers) { list.AddRange(servers); }

    public Server? RandomServer()
    {
        var ups = new List<Server>();
        for (int i = 0; i < list.Count; i++) if (list[i].up) ups.Add(list[i]);
        if (ups.Count == 0)
        {
            Console.WriteLine("No available Servers");      
            return null;
        }
        int index = Random.Shared.Next(ups.Count);
        return ups[index];
    }
}

class Program
{
    static async Task TcpClientHandler(TcpClient acceptedClient, ServerPool serverPool)
    {
        using (acceptedClient)
        {
            var chosenServer = serverPool.RandomServer();
            if (chosenServer is null)
            {
                return;
            }

            try
            {
                using var backendTcpClient = new TcpClient();

                var connectTask = backendTcpClient.ConnectAsync(chosenServer.host, chosenServer.port);
                var completedTask = await Task.WhenAny(connectTask, Task.Delay(800));
                if (completedTask != connectTask) 
                {
                    if (chosenServer.up)
                    {
                        Console.WriteLine($"Server DOWN: {chosenServer.host}:{chosenServer.port}");
                    }
                    chosenServer.up = false; 
                    return; 
                }
                await connectTask;

                using var clientNetworkStream = acceptedClient.GetStream();
                using var backendNetworkStream = backendTcpClient.GetStream();

                var clientToBackendCopyTask = clientNetworkStream.CopyToAsync(backendNetworkStream);
                var backendToClientCopyTask = backendNetworkStream.CopyToAsync(clientNetworkStream);

                await Task.WhenAny(clientToBackendCopyTask, backendToClientCopyTask);
            }
            catch
            {
                if (chosenServer.up)
                {
                    Console.WriteLine($"Server DOWN: {chosenServer.host}:{chosenServer.port}");
                }
                chosenServer.up = false;
            }
        }
    }

    static async Task DownServerRecheck(ServerPool serverPool)
    {
        while (true)
        {
            foreach (var s in serverPool.list)
            {
                if (s.up) continue;

                try
                {
                    using var probe = new TcpClient();
                    var connectTask = probe.ConnectAsync(s.host, s.port);
                    var done = await Task.WhenAny(connectTask, Task.Delay(800));
                    if (done == connectTask)
                    {
                        try
                        {
                            await connectTask; // confirm success
                            s.up = true;
                            Console.WriteLine($"Re-added {s.host}:{s.port}");
                        }
                        catch { }
                    }

                }
                catch { }
            }

            await Task.Delay(5000);
        }
    }

    static async Task Main()
    {
        var pool = new ServerPool(new[]
        {
            new Server("127.0.0.1", 9001),
            new Server("127.0.0.1", 9002),
            new Server("127.0.0.1", 9003),
        });

        foreach (var p in pool.list)
        {
            try
            {
                using var client = new TcpClient();
                var connectTask = client.ConnectAsync(p.host, p.port);
                var done = await Task.WhenAny(connectTask, Task.Delay(500));
                if (done == connectTask)
                {
                    try
                    {
                        await connectTask;
                        p.up = true;
                    }
                    catch { p.up = false; }
                }
                else
                {
                    p.up = false;
                }
            }
            catch
            {
                p.up = false;
                Console.WriteLine($"Server DOWN: {p.host}:{p.port}");
            }

        }

        _ = DownServerRecheck(pool);

        var listener = new TcpListener(IPAddress.Any, 9000);
        listener.Start();
        Console.WriteLine("Load Balancer listening on :9000 -> " + string.Join(", ", pool.list.ConvertAll(x => $"{x.host}:{x.port}")));
       
        while (true)
        {
            var client = await listener.AcceptTcpClientAsync();
            _ = TcpClientHandler(client, pool);
        }
    }

}