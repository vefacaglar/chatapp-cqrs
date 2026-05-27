using ChatApp.Domain.Entities.Command;

namespace ChatApp.Infrastructure.Transactions
{
    public class EventCreatedChatRoom : Event
    {
        public EventLog Data { get; set; } = default!;

        public EventCreatedChatRoom()
        {
        }

        public EventCreatedChatRoom(EventLog eventLog)
        {
            Data = eventLog;
        }
    }
}
