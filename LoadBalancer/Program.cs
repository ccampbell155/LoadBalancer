using LoadBalancer;

public class Program
{
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



 
