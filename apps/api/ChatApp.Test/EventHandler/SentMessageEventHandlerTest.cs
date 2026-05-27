using ChatApp.Application.EventHandler;
using ChatApp.Application.ReadModels;
using ChatApp.Application.Services;
using ChatApp.Infrastructure;
using ChatApp.Infrastructure.Transactions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;

namespace ChatApp.Test.EventHandler
{
    public class SentMessageEventHandlerTest
    {
        [Fact]
        public async Task Handle_UpdatesMongoAndPublishesToRedis()
        {
            var roomId = Guid.NewGuid();
            var evt = new MessageSentEvent(roomId, "Alice", "Hello World");

            var updateResultMock = new Mock<UpdateResult>();
            updateResultMock.Setup(x => x.MatchedCount).Returns(1);
            updateResultMock.Setup(x => x.ModifiedCount).Returns(1);

            var collectionMock = new Mock<IMongoCollection<ChatRoomReadModel>>();
            collectionMock
                .Setup(x => x.UpdateOneAsync(
                    It.IsAny<FilterDefinition<ChatRoomReadModel>>(),
                    It.IsAny<UpdateDefinition<ChatRoomReadModel>>(),
                    null,
                    default))
                .ReturnsAsync(updateResultMock.Object);

            var mongoServiceMock = new Mock<IMongoDbService>();
            mongoServiceMock.Setup(x => x.ChatRooms).Returns(collectionMock.Object);

            var redisPublisherMock = new Mock<IRedisPublisher>();
            var loggerMock = new Mock<ILogger<SentMessageEventHandler>>();

            var handler = new SentMessageEventHandler(
                mongoServiceMock.Object,
                redisPublisherMock.Object,
                loggerMock.Object);

            await handler.Handle(evt);

            collectionMock.Verify(
                x => x.UpdateOneAsync(
                    It.IsAny<FilterDefinition<ChatRoomReadModel>>(),
                    It.IsAny<UpdateDefinition<ChatRoomReadModel>>(),
                    null,
                    default),
                Times.Once);

            redisPublisherMock.Verify(
                x => x.PublishAsync(
                    $"chat:room:{roomId}:message",
                    It.Is<object>(o =>
                        o.GetType().GetProperty("RoomId")!.GetValue(o)!.Equals(roomId) &&
                        o.GetType().GetProperty("UserName")!.GetValue(o)!.Equals("Alice") &&
                        o.GetType().GetProperty("Message")!.GetValue(o)!.Equals("Hello World"))),
                Times.Once);
        }
    }
}
