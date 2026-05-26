using System.ComponentModel.DataAnnotations;

namespace ChatApp.Domain.Entities.Command
{
    public class ChatRoom : BaseEntity<Guid>, IAggregateRoot
    {
        [MaxLength(50)]
        public string Name { get; private set; }

        public ICollection<RoomMessage> Messages { get; private set; }

        public ChatRoom(string name) : base()
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Messages = new HashSet<RoomMessage>();
        }

        public void AddMessage(string userName, string message)
        {
            var roomMessage = new RoomMessage(userName, message);

            Messages.Add(roomMessage);
        }
    }
}
