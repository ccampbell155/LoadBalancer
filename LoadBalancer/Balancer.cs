using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LoadBalancer
{
    internal class Balancer
    {
        private readonly TcpListener balancer;
        public Balancer(string host, int port)
        {
            balancer = new TcpListener(IPAddress.Parse(host), port);
        }
        public async Task Start()
        {
            balancer.Start();
            Console.WriteLine("Load Balancer setup started");

            while (true)
            {
                using var client = await balancer.AcceptTcpClientAsync();
                await using var stream = client.GetStream();
                var message = Encoding.UTF8.GetBytes("Load Balancer Running");
                await stream.WriteAsync(message, 0, message.Length);
                await stream.FlushAsync();
            }
        }
    }
}
