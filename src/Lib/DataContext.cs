using AIApp.Lib.Entities;
using Microsoft.EntityFrameworkCore;

namespace AIApp.Lib
{
    public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
    {
        public DbSet<MemoryEntity> Memories { get; set; } = null!;
        public DbSet<ChatEntity> Chats { get; set; } = null!;
        public DbSet<MessageEntity> Messages { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MemoryEntity>().ToTable("Memories");
            modelBuilder.Entity<MemoryEntity>().HasBaseType<VectorEntity>();
            modelBuilder.Entity<MemoryEntity>().HasIndex(e => e.Subject).IsUnique();
            modelBuilder.Entity<MemoryEntity>().Property(e => e.Subject).IsRequired();
            modelBuilder.Entity<MemoryEntity>().Property(e => e.Content).IsRequired();

            modelBuilder.Entity<ChatEntity>().ToTable("Chats");
            modelBuilder.Entity<ChatEntity>().HasBaseType<BaseEntity>();
            modelBuilder.Entity<ChatEntity>().Property(e => e.Name).IsRequired();
            modelBuilder.Entity<ChatEntity>()
                .HasMany(e => e.Messages).WithOne(e => e.Chat).HasForeignKey(e => e.ChatId);

            modelBuilder.Entity<MessageEntity>().ToTable("Messages");
            modelBuilder.Entity<MessageEntity>().HasBaseType<VectorEntity>();
            modelBuilder.Entity<MessageEntity>().Property(e => e.Role).IsRequired();
            modelBuilder.Entity<MessageEntity>().Property(e => e.Content).IsRequired();
            modelBuilder.Entity<MessageEntity>().Property(e => e.ChatId).IsRequired();
        }
    }
}