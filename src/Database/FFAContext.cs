using FFA.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace FFA.Database
{
    public class FFAContext : DbContext
    {
        public DbSet<Poll> Polls { get; }
        public DbSet<Vote> Votes { get; }
        public DbSet<User> Users { get; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=FFA.db");
        }
    }
}
