using ChatApp.Application.Queries;
using ChatApp.Application.ReadModels;
using ChatApp.Application.Services;
using ChatApp.Test.Helpers;
using MongoDB.Driver;
using Moq;

namespace ChatApp.Test.Query
{
    public class GetChatRoomByIdQueryTest
    {
        [Fact]
        public async Task GetChatRoomById_WhenRoomExists_ReturnsRoom()
        {
            var roomId = Guid.NewGuid();
            var rooms = new List<ChatRoomReadModel>
            {
                new() { Id = roomId, Name = "Test Room" }
            };

            var collectionMock = new Mock<IMongoCollection<ChatRoomReadModel>>();
            MongoDbTestHelper.SetupCollectionFindAsync(collectionMock, rooms);

            var mongoServiceMock = new Mock<IMongoDbService>();
            mongoServiceMock.Setup(x => x.ChatRooms).Returns(collectionMock.Object);

            var handler = new GetChatRoomByIdQueryHandler(mongoServiceMock.Object);

            var result = await handler.HandleAsync(new GetChatRoomByIdQuery(roomId));

            Assert.NotNull(result);
            Assert.Equal(roomId, result!.Id);
            Assert.Equal("Test Room", result.Name);
        }

        [Fact]
        public async Task GetChatRoomById_WhenRoomDoesNotExist_ReturnsNull()
        {
            var collectionMock = new Mock<IMongoCollection<ChatRoomReadModel>>();
            MongoDbTestHelper.SetupCollectionFindAsync(collectionMock, new List<ChatRoomReadModel>());

            var mongoServiceMock = new Mock<IMongoDbService>();
            mongoServiceMock.Setup(x => x.ChatRooms).Returns(collectionMock.Object);

            var handler = new GetChatRoomByIdQueryHandler(mongoServiceMock.Object);

            var result = await handler.HandleAsync(new GetChatRoomByIdQuery(Guid.NewGuid()));

            Assert.Null(result);
        }
    }
}
