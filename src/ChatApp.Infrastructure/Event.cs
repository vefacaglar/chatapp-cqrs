namespace ChatApp.Infrastructure
{
    public class Event : IEvent
    {
        public Event()
        {
            Id = Guid.NewGuid();
            OccurredOn = DateTime.UtcNow;
        }

        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime OccurredOn { get; set; }
    }
}
