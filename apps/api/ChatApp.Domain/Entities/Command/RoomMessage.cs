namespace ChatApp.Domain.Entities.Command
{
    public class RoomMessage : BaseEntity<Guid>
    {
        public string Message { get; private set; }
        public string UserName { get; private set; }

        public RoomMessage(string userName, string message) : base()
        {
            Message = message;
            UserName = userName;
        }
    }
}
