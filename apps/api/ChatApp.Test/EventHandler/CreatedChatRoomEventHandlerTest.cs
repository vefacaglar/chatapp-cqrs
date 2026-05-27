using ChatApp.Application.EventHandler;
using ChatApp.Application.ReadModels;
using ChatApp.Application.Services;
using ChatApp.Domain.Entities.Command;
using ChatApp.Infrastructure;
using ChatApp.Infrastructure.Transactions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using Newtonsoft.Json;

namespace ChatApp.Test.EventHandler
{
    public class CreatedChatRoomEventHandlerTest
    {
        [Fact]
        public async Task Handle_WithValidPayload_InsertsIntoMongoAndPublishesToRedis()
        {
            var roomId = Guid.NewGuid();
            var chatRoom = new ChatRoomReadModel
            {
                Id = roomId,
                Name = "Test Room",
                CreatedAt = DateTime.UtcNow
            };
            var payload = JsonConvert.SerializeObject(chatRoom);

            var eventLog = new EventLog
            {
                Type = "CreatedChatRoom",
                Payload = payload
            };
            var evt = new EventCreatedChatRoom(eventLog);

            var collectionMock = new Mock<IMongoCollection<ChatRoomReadModel>>();
            var mongoServiceMock = new Mock<IMongoDbService>();
            mongoServiceMock.Setup(x => x.ChatRooms).Returns(collectionMock.Object);

            var redisPublisherMock = new Mock<IRedisPublisher>();
            var loggerMock = new Mock<ILogger<CreatedChatRoomEventHandler>>();

            var handler = new CreatedChatRoomEventHandler(
                mongoServiceMock.Object,
                redisPublisherMock.Object,
                loggerMock.Object);

            await handler.Handle(evt);

            collectionMock.Verify(
                x => x.InsertOneAsync(
                    It.Is<ChatRoomReadModel>(r => r.Id == roomId && r.Name == "Test Room"),
                    null,
                    default),
                Times.Once);

            redisPublisherMock.Verify(
                x => x.PublishAsync(
                    "chat:room:created",
                    It.Is<ChatRoomReadModel>(r => r.Id == roomId)),
                Times.Once);
        }

        [Fact]
        public async Task Handle_WithNullPayload_DoesNotInsertOrPublish()
        {
            var eventLog = new EventLog
            {
                Type = "CreatedChatRoom",
                Payload = "null"
            };
            var evt = new EventCreatedChatRoom(eventLog);

            var collectionMock = new Mock<IMongoCollection<ChatRoomReadModel>>();
            var mongoServiceMock = new Mock<IMongoDbService>();
            mongoServiceMock.Setup(x => x.ChatRooms).Returns(collectionMock.Object);

            var redisPublisherMock = new Mock<IRedisPublisher>();
            var loggerMock = new Mock<ILogger<CreatedChatRoomEventHandler>>();

            var handler = new CreatedChatRoomEventHandler(
                mongoServiceMock.Object,
                redisPublisherMock.Object,
                loggerMock.Object);

            await handler.Handle(evt);

            collectionMock.Verify(
                x => x.InsertOneAsync(It.IsAny<ChatRoomReadModel>(), null, default),
                Times.Never);
            redisPublisherMock.Verify(
                x => x.PublishAsync(It.IsAny<string>(), It.IsAny<object>()),
                Times.Never);
        }
    }
}
