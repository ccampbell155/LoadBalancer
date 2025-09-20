using System.Net;
using System.Net.Sockets;

namespace LoadBalancer.Tests
{
    public class TcpClientHandlerIntegrationTests
    {
        [Test]
        public async Task MarksDown_WhenConnectTimesOutOrFails()
        {
            var newListener = new TcpListener(IPAddress.Loopback, 0);
            newListener.Start();
            var dummyPortNumber = ((IPEndPoint)newListener.LocalEndpoint).Port;
            newListener.Stop();

            var backendServer = new Server("127.0.0.1", dummyPortNumber) { up = true };
            var serverPool = new ServerPool(new[] { backendServer });
            IServerSelector selector = new ServerSelector();
            
            using var dummyAcceptedClient = new TcpClient();

            await TcpClientHandler.ClientHandler(dummyAcceptedClient, serverPool, selector);

            Assert.IsFalse(backendServer.up);
        }
    }
}
