using ChatApp.Infrastructure.Database.Command;
using ChatApp.Infrastructure.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Repositories
{
    public class EfRepository<T> : IRepository<T> where T : class
    {
        protected readonly ChatDbContext _context;
        private readonly DbSet<T> _dbSet;

        public EfRepository(
            ChatDbContext context
        )
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public IQueryable<T> GetAll()
        {
            return _dbSet;
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public void Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }
    }
}
