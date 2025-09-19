using System.Net.Sockets;

namespace LoadBalancer
{
    public static class DownServerChecker
    {
        public static async Task DownServerRecheck(ServerPool serverPool)
        {
            while (true)
            {
                foreach (var s in serverPool.Servers)
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
    }
}
