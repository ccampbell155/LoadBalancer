using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LoadBalancer
{
    internal class Backend
    {
        private readonly TcpListener backEnd;
        public string Host { get; }
        public int Port { get; }
        public Backend(string host, int port)
        {
            Host = host;
            Port = port;
            backEnd = new TcpListener(IPAddress.Parse(Host), Port);
        }

        public async Task Start()
        {
            backEnd.Start();
            Console.WriteLine($"Backend is listening on {Host}:{Port}");

            while (true)
            {
                using var client = await backEnd.AcceptTcpClientAsync();
                await using var stream = client.GetStream();
                var message = Encoding.UTF8.GetBytes($"Backend Running on {Host}:{Port} ");
                await stream.WriteAsync(message, 0, message.Length);
                await stream.FlushAsync();
            }
        }
    }
}
