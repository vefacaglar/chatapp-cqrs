using ChatApp.Application.Queries;
using ChatApp.Application.ReadModels;
using ChatApp.Application.Services;
using ChatApp.Test.Helpers;
using MongoDB.Driver;
using Moq;

namespace ChatApp.Test.Query
{
    public class GetAllChatRoomsQueryTest
    {
        [Fact]
        public async Task GetAllChatRooms_WhenNoRooms_ReturnsEmptyList()
        {
            var collectionMock = new Mock<IMongoCollection<ChatRoomReadModel>>();
            MongoDbTestHelper.SetupCollectionFindAsync(collectionMock, new List<ChatRoomReadModel>());

            var mongoServiceMock = new Mock<IMongoDbService>();
            mongoServiceMock.Setup(x => x.ChatRooms).Returns(collectionMock.Object);

            var handler = new GetAllChatRoomsQueryHandler(mongoServiceMock.Object);

            var result = await handler.HandleAsync(new GetAllChatRoomsQuery());

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllChatRooms_WithRooms_ReturnsAllRooms()
        {
            var rooms = new List<ChatRoomReadModel>
            {
                new() { Id = Guid.NewGuid(), Name = "Room 1" },
                new() { Id = Guid.NewGuid(), Name = "Room 2" },
                new() { Id = Guid.NewGuid(), Name = "Room 3" }
            };

            var collectionMock = new Mock<IMongoCollection<ChatRoomReadModel>>();
            MongoDbTestHelper.SetupCollectionFindAsync(collectionMock, rooms);

            var mongoServiceMock = new Mock<IMongoDbService>();
            mongoServiceMock.Setup(x => x.ChatRooms).Returns(collectionMock.Object);

            var handler = new GetAllChatRoomsQueryHandler(mongoServiceMock.Object);

            var result = await handler.HandleAsync(new GetAllChatRoomsQuery());

            Assert.Equal(3, result.Count);
            Assert.Equal("Room 1", result[0].Name);
            Assert.Equal("Room 2", result[1].Name);
            Assert.Equal("Room 3", result[2].Name);
        }
    }
}
