using ChatApp.Application.ReadModels;
using ChatApp.Application.Services;
using ChatApp.Infrastructure;
using ChatApp.Infrastructure.Transactions;
using MongoDB.Driver;

namespace ChatApp.Application.EventHandler
{
    public class SentMessageEventHandler : IEventHandler<MessageSentEvent>
    {
        private readonly IMongoDbService _mongoDbService;
        private readonly IRedisPublisher _redisPublisher;

        public SentMessageEventHandler(IMongoDbService mongoDbService, IRedisPublisher redisPublisher)
        {
            _mongoDbService = mongoDbService;
            _redisPublisher = redisPublisher;
        }

        public async Task Handle(MessageSentEvent e)
        {
            var message = new RoomMessageReadModel
            {
                Id = Guid.NewGuid(),
                UserName = e.UserName,
                Message = e.Message,
                CreatedAt = DateTime.UtcNow
            };

            var filter = Builders<ChatRoomReadModel>.Filter.Eq(x => x.Id, e.RoomId);
            var update = Builders<ChatRoomReadModel>.Update.Push(x => x.Messages, message);

            await _mongoDbService.ChatRooms.UpdateOneAsync(filter, update);

            await _redisPublisher.PublishAsync($"chat:room:{e.RoomId}:message", new
            {
                RoomId = e.RoomId,
                message.Id,
                message.UserName,
                message.Message,
                message.CreatedAt
            });
        }
    }
}
