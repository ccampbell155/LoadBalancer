namespace LoadBalancer.UnitTests;

public class SelectorTests
{
    private IServerSelector _selector = null!;

    [SetUp]
    public void Setup() => _selector = new ServerSelector();


    [Test]
    public void SelectRandom_ReturnsOnlyUpServer()
    {
        var upServer = new Server("127.0.0.1", 10001) { up = true };
        var downServer1 = new Server("127.0.0.1", 10002) { up = false };
        var downServer2 = new Server("127.0.0.1", 10003) { up = false };
        var pool = new ServerPool(new[] { upServer, downServer1, downServer2 });
       
        var server = _selector.SelectRandom(pool.Servers);
        
        Assert.That(server, Is.Not.Null);
        Assert.That(server, Is.SameAs(upServer));
        
    }
    [Test]
    public void ReturnsNull_WhenNoServersAreUp()
    {
        var pool = new ServerPool(new[]
        {
            new Server("127.0.0.1", 10001) { up = false },
            new Server("127.0.0.1", 10002) { up = false },
        });

        Assert.That(_selector.SelectRandom(pool.Servers), Is.Null);
    }

    [Test]
    public void SelectRandom_WithTwoUps_SeesBothServers()
    {
        var a = new Server("127.0.0.1", 10001) { up = true };
        var b = new Server("127.0.0.1", 10002) { up = true };
        var pool = new ServerPool(new[] { a, b });
        
        var returnedA = false; 
        var returnedB = false;

        for (int i = 0; i < 50; i++)
        {
            var chosenServer = _selector.SelectRandom(pool.Servers);
            if (ReferenceEquals(chosenServer, a)) returnedA = true;
            if (ReferenceEquals(chosenServer, b)) returnedB = true;
        }

        Assert.That(returnedA, Is.True);
        Assert.That(returnedB, Is.True);
    }
}