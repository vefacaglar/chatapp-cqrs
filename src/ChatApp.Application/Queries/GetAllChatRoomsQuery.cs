using ChatApp.Application.ReadModels;
using ChatApp.Application.Services;
using CustomDispatcher.Abstractions.Queries;
using MongoDB.Driver;

namespace ChatApp.Application.Queries
{
    public class GetAllChatRoomsQuery : IQuery<List<ChatRoomReadModel>>
    {
    }

    public class GetAllChatRoomsQueryHandler : IQueryProcessor<GetAllChatRoomsQuery, List<ChatRoomReadModel>>
    {
        private readonly IMongoDbService _mongoDbService;

        public GetAllChatRoomsQueryHandler(IMongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }

        public async Task<List<ChatRoomReadModel>> HandleAsync(GetAllChatRoomsQuery query, CancellationToken cancellationToken = default)
        {
            var filter = Builders<ChatRoomReadModel>.Filter.Empty;
            var result = await _mongoDbService.ChatRooms.Find(filter).ToListAsync(cancellationToken);
            return result;
        }
    }
}
