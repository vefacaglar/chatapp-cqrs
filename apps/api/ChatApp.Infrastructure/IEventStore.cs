using ChatApp.Domain.Entities.Command;
using Newtonsoft.Json;

namespace ChatApp.Infrastructure
{
    public interface IEventStore
    {
        Task Save<T>(T data, string type);
    }

    public class SqlEventStore : IEventStore
    {
        private readonly IUnitOfWork _uow;

        public SqlEventStore(
            IUnitOfWork uow
            )
        {
            _uow = uow;
        }

        public async Task Save<T>(T data, string type)
        {
            var repository = _uow.GetRepository<EventLog>();

            var eventLog = new EventLog()
            {
                Type = type,
                Payload = JsonConvert.SerializeObject(data),
                CreatedAt = DateTime.UtcNow,
            };

            repository.Add(eventLog);
            await _uow.SaveChangesAsync();
        }
    }
}
