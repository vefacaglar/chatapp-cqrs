using Microsoft.Extensions.DependencyInjection;

namespace ChatApp.Infrastructure
{
    public class EventDispatcher : IEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public EventDispatcher(
            IServiceProvider serviceProvider
            )
        {
            _serviceProvider = serviceProvider;
        }

        public Task Dispatch<TEvent>(TEvent e) where TEvent : IEvent
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            var eventType = typeof(IEventHandler<>).MakeGenericType(e.GetType());

            var handler = _serviceProvider.GetService(eventType);

            if (handler == null)
            {
                throw new InvalidOperationException($"No handler registered for event type {e.GetType().Name}");
            }

            return (Task)eventType
                .GetMethod("Handle")!
                .Invoke(handler, new object[] { e })!;
        }
    }
}
