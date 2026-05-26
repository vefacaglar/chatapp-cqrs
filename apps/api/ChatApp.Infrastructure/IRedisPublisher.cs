namespace ChatApp.Infrastructure
{
    public interface IRedisPublisher
    {
        Task PublishAsync(string channel, object message);
    }
}
