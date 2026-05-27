using ChatApp.Application.ReadModels;
using MongoDB.Driver;

namespace ChatApp.Application.Services
{
    public interface IMongoDbService
    {
        IMongoCollection<ChatRoomReadModel> ChatRooms { get; }
    }
}
