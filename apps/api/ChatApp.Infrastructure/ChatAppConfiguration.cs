namespace ChatApp.Infrastructure
{
    public class ChatAppConfiguration
    {
        public int RetryCount { get; set; }
        public required ConnectionStrings ConnectionStrings { get; set; }
        public required MongoDbSettings MongoDb { get; set; }
        public required RedisSettings Redis { get; set; }
    }

    public class ConnectionStrings
    {
        public required string ChatDbCommand { get; set; }
        public required EventBusSetting EventBus { get; set; }
    }

    public class EventBusSetting
    {
        public required string Connection { get; set; }
        public required string UserName { get; set; }
        public required string Password { get; set; }
    }

    public class MongoDbSettings
    {
        public required string ConnectionString { get; set; }
        public required string DatabaseName { get; set; }
    }

    public class RedisSettings
    {
        public required string ConnectionString { get; set; }
    }
}
