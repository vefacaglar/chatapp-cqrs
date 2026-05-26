using ChatApp.Domain.Entities.Command;

namespace ChatApp.Infrastructure.Repositories.Abstractions
{
    public interface IChatRepository : IRepository<ChatRoom>
    {
        Task<ChatRoom?> GetByIdAsync(Guid id);
    }
}
