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

        public CreatedChatRoomEventHandler(IMongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }

        public async Task Handle(EventCreatedChatRoom e)
        {
            var chatRoom = JsonConvert.DeserializeObject<ChatRoomReadModel>(e.Data.Payload);

            if (chatRoom != null)
            {
                await _mongoDbService.ChatRooms.InsertOneAsync(chatRoom);
            }
        }
    }
}
