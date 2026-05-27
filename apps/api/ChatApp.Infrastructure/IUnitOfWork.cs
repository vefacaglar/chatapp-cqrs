using ChatApp.Infrastructure.Repositories.Abstractions;
using ChatApp.Infrastructure.Repositories;
using ChatApp.Infrastructure.Database.Command;

namespace ChatApp.Infrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> GetRepository<T>() where T : class;
        Task SaveChangesAsync();
    }

    public class ChatDbUnitOfWork : IUnitOfWork
    {
        private readonly ChatDbContext _dbContext;

        public ChatDbUnitOfWork(
            ChatDbContext context
            )
        {
            _dbContext = context;
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public IRepository<T> GetRepository<T>() where T : class
        {
            return new EfRepository<T>(_dbContext);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
