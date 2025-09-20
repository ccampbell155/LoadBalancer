using System.Net.Sockets;

namespace LoadBalancer
{
    public static class TcpClientHandler
    {
        public static async Task ClientHandler(TcpClient acceptedClient, ServerPool serverPool, IServerSelector selector)
        {
            using (acceptedClient)
            {
                var chosenServer = selector.SelectRandom(serverPool.Servers);
                if (chosenServer is null)
                {
                    return;
                }

                try
                {
                    using var backendTcpClient = new TcpClient();

                    var connectTask = backendTcpClient.ConnectAsync(chosenServer.host, chosenServer.port);
                    var completedTask = await Task.WhenAny(connectTask, Task.Delay(500));
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
    }
}
