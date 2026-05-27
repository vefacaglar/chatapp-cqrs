using ChatApp.Application.ReadModels;
using ChatApp.Application.Services;
using ChatApp.Infrastructure;
using ChatApp.Infrastructure.Transactions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ChatApp.Application.EventHandler
{
    public class CreatedChatRoomEventHandler : IEventHandler<EventCreatedChatRoom>
    {
        private readonly IMongoDbService _mongoDbService;
        private readonly IRedisPublisher _redisPublisher;
        private readonly ILogger<CreatedChatRoomEventHandler> _logger;

        public CreatedChatRoomEventHandler(IMongoDbService mongoDbService, IRedisPublisher redisPublisher, ILogger<CreatedChatRoomEventHandler> logger)
        {
            _mongoDbService = mongoDbService;
            _redisPublisher = redisPublisher;
            _logger = logger;
        }

        public async Task Handle(EventCreatedChatRoom e)
        {
            _logger.LogInformation("Processing EventCreatedChatRoom: Payload={Payload}", e.Data.Payload);

            var chatRoom = JsonConvert.DeserializeObject<ChatRoomReadModel>(e.Data.Payload);

            if (chatRoom != null)
            {
                _logger.LogInformation("Inserting room into MongoDB: Id={Id}, Name={Name}", chatRoom.Id, chatRoom.Name);
                await _mongoDbService.ChatRooms.InsertOneAsync(chatRoom);
                await _redisPublisher.PublishAsync("chat:room:created", chatRoom);
            }
            else
            {
                _logger.LogWarning("Failed to deserialize ChatRoomReadModel from payload");
            }
        }
    }
}
