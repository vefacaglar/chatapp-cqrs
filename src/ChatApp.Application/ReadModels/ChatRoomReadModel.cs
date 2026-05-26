using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ChatApp.Application.ReadModels
{
    public class ChatRoomReadModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public List<RoomMessageReadModel> Messages { get; set; } = new();
    }

    public class RoomMessageReadModel
    {
        public Guid Id { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
