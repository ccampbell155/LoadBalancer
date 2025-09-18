using System.Net;
using System.Net.Sockets;

namespace LoadBalancer
{
    internal class Balancer
    {
        private readonly TcpListener balancerListener;
        private readonly Backend backend;
        public Balancer(string balancerHost, int balancerPort, Backend backend)
        {
            balancerListener = new TcpListener(IPAddress.Parse(balancerHost), balancerPort);
            this.backend = backend;
        }
        public async Task Start()
        {
            balancerListener.Start();
            Console.WriteLine($"Load Balancer is listening on {balancerListener.LocalEndpoint}");

            while (true)
            {
                var client = await balancerListener.AcceptTcpClientAsync();
                _ = HandleConnection(client);
            }
        }

        private async Task HandleConnection(TcpClient incomingClient)
        {
            using (incomingClient)
            {
                try
                {
                    using var backendConnection = new TcpClient();
                    await backendConnection.ConnectAsync(backend.Host, backend.Port);

                    using var clientStream = incomingClient.GetStream();
                    using var backendStream = backendConnection.GetStream();

                    var clientToBackend = clientStream.CopyToAsync(backendStream);
                    var backendToClient = backendStream.CopyToAsync(clientStream);

                    await Task.WhenAny(clientToBackend, backendToClient);
                }
                catch (Exception ex) {
                    Console.WriteLine($"Error: { ex.Message}");
                }
            }
        }

    }
}
