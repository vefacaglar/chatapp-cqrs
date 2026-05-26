using ChatApp.Application.ReadModels;
using ChatApp.Infrastructure;
using MongoDB.Driver;

namespace ChatApp.Application.Services
{
    public class MongoDbService : IMongoDbService
    {
        private readonly IMongoDatabase _database;

        public MongoDbService(ChatAppConfiguration configuration)
        {
            var client = new MongoClient(configuration.MongoDb.ConnectionString);
            _database = client.GetDatabase(configuration.MongoDb.DatabaseName);
        }

        public IMongoCollection<ChatRoomReadModel> ChatRooms =>
            _database.GetCollection<ChatRoomReadModel>("ChatRooms");
    }
}
