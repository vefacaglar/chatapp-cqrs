using ChatApp.Domain.Entities.Command;
using ChatApp.Infrastructure.Database.Command;
using ChatApp.Infrastructure.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Repositories
{
    public class ChatRepository : EfRepository<ChatRoom>, IChatRepository
    {
        public ChatRepository(ChatDbContext context) : base(context)
        {
        }

        public async Task<ChatRoom?> GetByIdAsync(Guid id)
        {
            var room = await _context.FindAsync<ChatRoom>(id);
            return room;
        }
    }
}
