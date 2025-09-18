using System.Net;
using System.Net.Sockets;
using System.Text;

namespace LoadBalancer
{
    internal class Backend
    {
        private readonly TcpListener backEnd;
        public Backend(string host, int port)
        {
            backEnd = new TcpListener(IPAddress.Parse(host), port);
        }

        public async Task Start()
        {
            backEnd.Start();
            Console.WriteLine("Backend setup started");

            while (true)
            {
                using var client = await backEnd.AcceptTcpClientAsync();
                await using var stream = client.GetStream();
                var message = Encoding.UTF8.GetBytes("Backend Running");
                await stream.WriteAsync(message, 0, message.Length);
                await stream.FlushAsync();
            }
        }
    }
}
