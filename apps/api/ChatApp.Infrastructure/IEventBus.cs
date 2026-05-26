namespace ChatApp.Infrastructure
{
    public interface IEventBus
    {
        void Publish(IEvent @event);
        void Subscribe<T>() where T : IEvent;
    }
}
