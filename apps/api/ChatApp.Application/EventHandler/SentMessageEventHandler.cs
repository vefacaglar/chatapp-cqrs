using ChatApp.Application.ReadModels;
using ChatApp.Application.Services;
using ChatApp.Infrastructure;
using ChatApp.Infrastructure.Transactions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace ChatApp.Application.EventHandler
{
    public class SentMessageEventHandler : IEventHandler<MessageSentEvent>
    {
        private readonly IMongoDbService _mongoDbService;
        private readonly IRedisPublisher _redisPublisher;
        private readonly ILogger<SentMessageEventHandler> _logger;

        public SentMessageEventHandler(IMongoDbService mongoDbService, IRedisPublisher redisPublisher, ILogger<SentMessageEventHandler> logger)
        {
            _mongoDbService = mongoDbService;
            _redisPublisher = redisPublisher;
            _logger = logger;
        }

        public async Task Handle(MessageSentEvent e)
        {
            _logger.LogInformation("Processing MessageSentEvent: RoomId={RoomId}, UserName={UserName}, Message={Message}", e.RoomId, e.UserName, e.Message);

            var message = new RoomMessageReadModel
            {
                Id = Guid.NewGuid(),
                UserName = e.UserName,
                Message = e.Message,
                CreatedAt = DateTime.UtcNow
            };

            var filter = Builders<ChatRoomReadModel>.Filter.Eq(x => x.Id, e.RoomId);
            var update = Builders<ChatRoomReadModel>.Update.Push(x => x.Messages, message);

            var result = await _mongoDbService.ChatRooms.UpdateOneAsync(filter, update);
            _logger.LogInformation("MongoDB UpdateOne result: MatchedCount={MatchedCount}, ModifiedCount={ModifiedCount}", result.MatchedCount, result.ModifiedCount);

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
