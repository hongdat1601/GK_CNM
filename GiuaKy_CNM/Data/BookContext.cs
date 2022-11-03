using GiuaKy_CNM.Models;
using Microsoft.EntityFrameworkCore;

namespace GiuaKy_CNM.Data
{
    public class BookContext : DbContext
    {
        public BookContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Book> Books { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Book>()
                .Property(b => b.Author)
                .HasDefaultValue("Unknown");

            modelBuilder.Entity<Book>()
                .Property(b => b.PublishDate);

            modelBuilder.Entity<Book>()
                .Property(b => b.Categories)
                .HasDefaultValue("Unknown");
        }
    }
}
