namespace ChatApp.Infrastructure
{
    public interface IEventDispatcher
    {
        Task Dispatch<TEvent>(TEvent e) where TEvent : IEvent;
    }
}
