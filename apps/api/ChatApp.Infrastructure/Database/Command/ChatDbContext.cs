using ChatApp.Domain.Entities.Command;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Database.Command
{
    public partial class ChatDbContext : DbContext
    {
        public ChatDbContext()
        {

        }

        public ChatDbContext(DbContextOptions<ChatDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ChatRoom> ChatRoom { get; set; }
        public virtual DbSet<EventLog> EventLog { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
