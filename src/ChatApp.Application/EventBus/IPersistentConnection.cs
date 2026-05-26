namespace ChatApp.Application.EventBus
{
    public interface IPersistentConnection<T> : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();

        T CreateChannel();
    }
}
