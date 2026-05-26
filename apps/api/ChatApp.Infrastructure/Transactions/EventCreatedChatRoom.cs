using ChatApp.Domain.Entities.Command;

namespace ChatApp.Infrastructure.Transactions
{
    public class EventCreatedChatRoom : Event
    {
        public EventLog Data { get; set; }

        public EventCreatedChatRoom(EventLog eventLog)
        {
            Data = eventLog;
        }
    }
}
