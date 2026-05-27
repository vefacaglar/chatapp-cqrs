using ChatApp.Application.Chat;
using ChatApp.Domain.Entities.Command;
using ChatApp.Infrastructure;
using ChatApp.Infrastructure.Repositories.Abstractions;
using ChatApp.Infrastructure.Transactions;
using Moq;

namespace ChatApp.Test.Command
{
    public class SendMessageTest
    {
        private readonly SendMessageCommandHandler _handler;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IChatRepository> _chatRepository;
        private readonly Mock<IEventBus> _eventBus;

        public SendMessageTest()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _chatRepository = new Mock<IChatRepository>();
            _eventBus = new Mock<IEventBus>();

            _handler = new SendMessageCommandHandler(
                _unitOfWork.Object,
                _chatRepository.Object,
                _eventBus.Object);
        }

        [Fact]
        public async Task SendMessage_WhenRoomNotFound_ReturnsFalse()
        {
            var roomId = Guid.NewGuid();
            _chatRepository
                .Setup(x => x.GetByIdAsync(roomId))
                .ReturnsAsync((ChatRoom?)null);

            var command = new SendMessageCommand(roomId, "Hello", "Alice");

            var result = await _handler.HandleAsync(command);

            Assert.False(result.Success);
            _eventBus.Verify(x => x.Publish(It.IsAny<IEvent>()), Times.Never);
        }

        [Fact]
        public async Task SendMessage_WhenRoomFound_ReturnsTrue()
        {
            var roomId = Guid.NewGuid();
            var room = new ChatRoom("Test Room");
            _chatRepository
                .Setup(x => x.GetByIdAsync(roomId))
                .ReturnsAsync(room);

            var command = new SendMessageCommand(roomId, "Hello", "Alice");

            var result = await _handler.HandleAsync(command);

            Assert.True(result.Success);
        }

        [Fact]
        public async Task SendMessage_WhenRoomFound_AddsMessageToRoom()
        {
            var roomId = Guid.NewGuid();
            var room = new ChatRoom("Test Room");
            _chatRepository
                .Setup(x => x.GetByIdAsync(roomId))
                .ReturnsAsync(room);

            var command = new SendMessageCommand(roomId, "Hello", "Alice");

            await _handler.HandleAsync(command);

            Assert.Single(room.Messages);
            Assert.Equal("Hello", room.Messages.First().Message);
            Assert.Equal("Alice", room.Messages.First().UserName);
        }

        [Fact]
        public async Task SendMessage_WhenRoomFound_SavesAndPublishesEvent()
        {
            var roomId = Guid.NewGuid();
            var room = new ChatRoom("Test Room");
            _chatRepository
                .Setup(x => x.GetByIdAsync(roomId))
                .ReturnsAsync(room);

            var command = new SendMessageCommand(roomId, "Hello", "Alice");

            await _handler.HandleAsync(command);

            _unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
            _eventBus.Verify(
                x => x.Publish(It.Is<MessageSentEvent>(e =>
                    e.RoomId == roomId &&
                    e.UserName == "Alice" &&
                    e.Message == "Hello")),
                Times.Once);
        }
    }
}
