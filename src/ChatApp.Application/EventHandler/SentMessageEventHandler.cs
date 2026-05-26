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

        public SentMessageEventHandler(IMongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
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
        }
    }
}
