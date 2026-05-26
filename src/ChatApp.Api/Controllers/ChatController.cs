using ChatApp.Application.Chat;
using ChatApp.Domain;
using ChatApp.Domain.Chat.Request;
using ChatApp.Domain.Chat.Response;
using CustomDispatcher.Abstractions.Dispatching;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly ICommandDispatcher _dispatcher;

        public ChatController(
            ICommandDispatcher dispatcher
            )
        {
            _dispatcher = dispatcher;
        }

        [HttpPost]
        public async Task<ActionResult<CreateChatRoomCommandResult>> CreateChatRoomAsync(CreateChatRoomRequest request)
        {
            var command = new CreateChatRoomCommand(request.Name);

            var result = await _dispatcher.DispatchAsync<CreateChatRoomCommand, CreateChatRoomCommandResult>(command);

            return Ok(result);
        }

        [HttpPost("message")]
        public async Task<ActionResult<SendMessageCommandResult>> SendMessageAsync(SendMessageRequest request)
        {
            var command = new SendMessageCommand(request.RoomId, request.Message, request.UserName);
            var result = await _dispatcher.DispatchAsync<SendMessageCommand, SendMessageCommandResult>(command);
            return Ok(result);
        }
    }
}
