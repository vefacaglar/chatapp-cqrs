using CustomDispatcher.Abstractions.Commands;
using ChatApp.Domain.Entities.Command;
using ChatApp.Infrastructure;
using ChatApp.Infrastructure.Repositories.Abstractions;

namespace ChatApp.Application.Chat
{
    public class SendMessageCommand : ICommand<SendMessageCommandResult>
    {
        public Guid RoomId { get; private set; }
        public string Message { get; private set; }
        public string UserName { get; private set; }

        public SendMessageCommand(
            Guid roomId, string message, string userName
            )
        {
            RoomId = roomId;
            Message = message;
            UserName = userName;
        }
    }

    public class SendMessageCommandResult
    {
        public bool Success { get; set; }

        public SendMessageCommandResult()
        {
        }

        public SendMessageCommandResult(bool success)
        {
            Success = success;
        }
    }

    public sealed class SendMessageCommandHandler : ICommandProcessor<SendMessageCommand, SendMessageCommandResult>
    {
        private readonly IUnitOfWork _uow;
        private readonly IChatRepository _chatRepository;
        private readonly IEventBus _eventBus;

        public SendMessageCommandHandler(
            IUnitOfWork uow,
            IChatRepository chatRepository,
            IEventBus eventBus
            )
        {
            _uow = uow;
            _chatRepository = chatRepository;
            _eventBus = eventBus;
        }

        public async Task<SendMessageCommandResult> HandleAsync(SendMessageCommand command, CancellationToken cancellationToken = default)
        {
            var room = await _chatRepository.GetByIdAsync(command.RoomId);
            room.AddMessage(command.UserName, command.Message);
            await _uow.SaveChangesAsync();

            return new SendMessageCommandResult(true);
        }
    }
}
