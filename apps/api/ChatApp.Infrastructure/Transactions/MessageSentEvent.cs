namespace ChatApp.Infrastructure.Transactions
{
    public class MessageSentEvent : Event
    {
        public Guid RoomId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        public MessageSentEvent(Guid roomId, string userName, string message)
        {
            RoomId = roomId;
            UserName = userName;
            Message = message;
        }
    }
}
