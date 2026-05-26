namespace ChatApp.Infrastructure
{
    public interface IEventHandler<TEvent> where TEvent : IEvent
    {
        Task Handle(TEvent e);
    }
}