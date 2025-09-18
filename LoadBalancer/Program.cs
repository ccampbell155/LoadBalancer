using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using LoadBalancer;

public class Program
{
    private readonly string backendHost;
    private readonly string balancerHost;
    private readonly int backendPort;
    private readonly int balancerPort;
    private readonly Backend backend;
    private readonly Balancer balancer;

    public Program(string backEndHost, int backEndport, string balancerHost, int balancerPort)
    {
        backend = new Backend(backEndHost, backEndport);
        balancer = new Balancer(balancerHost, balancerPort);
    }

    public async Task Run()
    {
        _ = backend.Start();
        await balancer.Start();
    }

    public static async Task Main()
    {
        var program = new Program("127.0.0.1", 9001, "127.0.0.1", 9000);
        await program.Run();
    }

}



 
