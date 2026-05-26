using ChatApp.Application.ReadModels;
using ChatApp.Application.Services;
using ChatApp.Infrastructure;
using ChatApp.Infrastructure.Transactions;
using Newtonsoft.Json;

namespace ChatApp.Application.EventHandler
{
    public class CreatedChatRoomEventHandler : IEventHandler<EventCreatedChatRoom>
    {
        private readonly IMongoDbService _mongoDbService;
        private readonly IRedisPublisher _redisPublisher;

        public CreatedChatRoomEventHandler(IMongoDbService mongoDbService, IRedisPublisher redisPublisher)
        {
            _mongoDbService = mongoDbService;
            _redisPublisher = redisPublisher;
        }

        public async Task Handle(EventCreatedChatRoom e)
        {
            var chatRoom = JsonConvert.DeserializeObject<ChatRoomReadModel>(e.Data.Payload);

            if (chatRoom != null)
            {
                await _mongoDbService.ChatRooms.InsertOneAsync(chatRoom);
                await _redisPublisher.PublishAsync("chat:room:created", chatRoom);
            }
        }
    }
}
