using ChatApp.Application.Chat;
using ChatApp.Domain.Entities.Command;
using ChatApp.Infrastructure;
using ChatApp.Infrastructure.Repositories.Abstractions;
using ChatApp.Infrastructure.Transactions;
using Moq;

namespace ChatApp.Test.Command
{
    public class ChatRoomTest
    {
        private readonly CreateChatRoomCommandHandler _handler;
        private readonly Mock<IEventBus> _eventBus;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IRepository<ChatRoom>> _chatRoomRepository;

        public ChatRoomTest()
        {
            _eventBus = new Mock<IEventBus>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _chatRoomRepository = new Mock<IRepository<ChatRoom>>();
            _chatRoomRepository.Setup(x => x.Add(It.IsAny<ChatRoom>()));
            _unitOfWork.Setup(x => x.GetRepository<ChatRoom>()).Returns(_chatRoomRepository.Object);

            _handler = new CreateChatRoomCommandHandler(_unitOfWork.Object, _eventBus.Object);
        }

        [Fact]
        public async Task CreateChatRoom_WithName_MustHaveCode()
        {
            var command = new CreateChatRoomCommand("test");

            var result = await _handler.HandleAsync(command);

            Assert.NotEmpty(result.Code);
        }

        [Fact]
        public async Task CreateChatRoom_WithName_ReturnsCorrectName()
        {
            var command = new CreateChatRoomCommand("My Room");

            var result = await _handler.HandleAsync(command);

            Assert.Equal("My Room", result.Name);
        }

        [Fact]
        public async Task CreateChatRoom_SavesToRepository()
        {
            var command = new CreateChatRoomCommand("test");

            await _handler.HandleAsync(command);

            _unitOfWork.Verify(x => x.GetRepository<ChatRoom>(), Times.Once);
            _chatRoomRepository.Verify(x => x.Add(It.IsAny<ChatRoom>()), Times.Once);
            _unitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateChatRoom_PublishesEventCreatedChatRoom()
        {
            var command = new CreateChatRoomCommand("test");

            await _handler.HandleAsync(command);

            _eventBus.Verify(
                x => x.Publish(It.Is<EventCreatedChatRoom>(e => e.Data.Type == "CreatedChatRoom")),
                Times.Once);
        }
    }
}
