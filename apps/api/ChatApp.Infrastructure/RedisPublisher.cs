using Newtonsoft.Json;
using StackExchange.Redis;

namespace ChatApp.Infrastructure
{
    public class RedisPublisher : IRedisPublisher
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public RedisPublisher(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
        }

        public async Task PublishAsync(string channel, object message)
        {
            var subscriber = _connectionMultiplexer.GetSubscriber();
            var json = JsonConvert.SerializeObject(message);
            await subscriber.PublishAsync(RedisChannel.Literal(channel), json);
        }
    }
}
