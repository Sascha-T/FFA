using FFA.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FFA.Database
{
    public class FFAContext : DbContext
    {
        public DbSet<Poll> Polls { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Rule> Rules { get; set; }
        public DbSet<Mute> Mutes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=FFA.db");
        }

        // Generic methods
        public async Task AddAsync<T>(T entity) where T : class
        {
            await Set<T>().AddAsync(entity);
            await SaveChangesAsync();
        }

        public async Task<T> GetAsync<T>(object key, Action<T> factory) where T : class, new()
        {
            var set = Set<T>();
            var entity = await set.FindAsync(key);

            if (entity == null)
            {
                entity = new T();

                factory(entity);

                await set.AddAsync(entity);
                await SaveChangesAsync();
            }

            return entity;
        }

        public async Task RemoveAsync<T>(object key) where T : class
        {
            var set = Set<T>();
            set.Remove(await set.FindAsync(key));
            await SaveChangesAsync();
        }

        public async Task RemoveAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            var set = Set<T>();
            set.Remove(await set.FirstOrDefaultAsync(predicate));
            await SaveChangesAsync();
        }

        public async Task UpsertAsync<T>(object key, Action<T> factory, Action<T> update) where T : class, new()
        {
            var entity = await GetAsync(key, factory);

            update(entity);

            Set<T>().Update(entity);
            await SaveChangesAsync();
        }

        public async Task UpdateAsync<T>(Expression<Func<T, bool>> predicate, Action<T> update) where T : class, new()
        {
            var set = Set<T>();
            var entity = await set.FirstAsync(predicate);

            update(entity);

            set.Update(entity);
            await SaveChangesAsync();
        }

        // User methods
        public Task<User> GetUserAsync(ulong id)
            => GetAsync<User>(id, (x) => x.Id = id);

        public Task UpsertUserAsync(ulong id, Action<User> update)
            => UpsertAsync(id, (x) => x.Id = id, update);
    }
}
