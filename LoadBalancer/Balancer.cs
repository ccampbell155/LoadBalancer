using LoadBalancer;
using System.Net;
using System.Net.Sockets;

class Program
{
    static readonly IServerSelector selector = new ServerSelector();

    static async Task Main()
    {
        var pool = new ServerPool(new[]
        {
            new Server("127.0.0.1", 9001),
            new Server("127.0.0.1", 9002),
            new Server("127.0.0.1", 9003),
        });

        foreach (var p in pool.Servers)
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

        _ = DownServerChecker.DownServerRecheck(pool);

        var listener = new TcpListener(IPAddress.Any, 9000);
        listener.Start();
        Console.WriteLine("Load Balancer listening on :9000 -> " + string.Join(", ", pool.Servers.Select(s => $"{s.host}:{s.port}")));


        while (true)
        {
            var client = await listener.AcceptTcpClientAsync();
            _ = TcpClientHandler.ClientHandler(client, pool, selector);
        }
    }

}