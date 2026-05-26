using ChatApp.Application.ReadModels;
using ChatApp.Application.Services;
using CustomDispatcher.Abstractions.Queries;
using MongoDB.Driver;

namespace ChatApp.Application.Queries
{
    public class GetChatRoomByIdQuery : IQuery<ChatRoomReadModel?>
    {
        public Guid RoomId { get; set; }

        public GetChatRoomByIdQuery(Guid roomId)
        {
            RoomId = roomId;
        }
    }

    public class GetChatRoomByIdQueryHandler : IQueryProcessor<GetChatRoomByIdQuery, ChatRoomReadModel?>
    {
        private readonly IMongoDbService _mongoDbService;

        public GetChatRoomByIdQueryHandler(IMongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }

        public async Task<ChatRoomReadModel?> HandleAsync(GetChatRoomByIdQuery query, CancellationToken cancellationToken = default)
        {
            var filter = Builders<ChatRoomReadModel>.Filter.Eq(x => x.Id, query.RoomId);
            var result = await _mongoDbService.ChatRooms.Find(filter).FirstOrDefaultAsync(cancellationToken);
            return result;
        }
    }
}
