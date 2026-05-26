using ChatApp.Application.Chat;
using ChatApp.Application.Queries;
using ChatApp.Application.ReadModels;
using ChatApp.Domain.Chat.Request;
using CustomDispatcher.Abstractions.Dispatching;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;

        public ChatController(
            ICommandDispatcher commandDispatcher,
            IQueryDispatcher queryDispatcher
            )
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
        }

        [HttpPost]
        public async Task<ActionResult<CreateChatRoomCommandResult>> CreateChatRoomAsync(CreateChatRoomRequest request)
        {
            var command = new CreateChatRoomCommand(request.Name);

            var result = await _commandDispatcher.DispatchAsync<CreateChatRoomCommand, CreateChatRoomCommandResult>(command);

            return Ok(result);
        }

        [HttpPost("message")]
        public async Task<ActionResult<SendMessageCommandResult>> SendMessageAsync(SendMessageRequest request)
        {
            var command = new SendMessageCommand(request.RoomId, request.Message, request.UserName);
            var result = await _commandDispatcher.DispatchAsync<SendMessageCommand, SendMessageCommandResult>(command);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<ChatRoomReadModel>>> GetAllChatRoomsAsync()
        {
            var query = new GetAllChatRoomsQuery();
            var result = await _queryDispatcher.DispatchAsync<GetAllChatRoomsQuery, List<ChatRoomReadModel>>(query);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ChatRoomReadModel>> GetChatRoomByIdAsync(Guid id)
        {
            var query = new GetChatRoomByIdQuery(id);
            var result = await _queryDispatcher.DispatchAsync<GetChatRoomByIdQuery, ChatRoomReadModel?>(query);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}
