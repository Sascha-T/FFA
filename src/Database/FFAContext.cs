using FFA.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace FFA.Database
{
    public class FFAContext : DbContext
    {
        // TODO: add mutes
        public DbSet<Poll> Polls { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Rule> Rules { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=FFA.db");
        }

        public async Task<User> GetUser(ulong id)
        {
            var user = await Users.FindAsync(id);

            if (user == null)
            {
                user = new User()
                {
                    Id = id
                };

                await Users.AddAsync(user);
                await SaveChangesAsync();
            }

            return user;
        }

        public async Task UpdateUser(ulong id, Action<User> update)
        {
            var user = await GetUser(id);

            update(user);

            Users.Update(user);
            await SaveChangesAsync();
        }
    }
}
