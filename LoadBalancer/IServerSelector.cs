namespace LoadBalancer
{
    public interface IServerSelector
    {
        Server? SelectRandom(IReadOnlyList<Server> servers);
    }
}
