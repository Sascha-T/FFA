using Discord;
using FFA.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FFA.Database
{
    public class FFAContext : DbContext
    {
        public DbSet<Guild> Guilds { get; set; }
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

        public Task RemoveAsync<T>(T entity) where T : class
        {
            Set<T>().Remove(entity);
            return SaveChangesAsync();
        }

        public async Task RemoveAsync<T>(object key) where T : class
            => await RemoveAsync(await Set<T>().FindAsync(key));

        public async Task RemoveAsync<T>(Expression<Func<T, bool>> predicate) where T : class
            => await RemoveAsync(await Set<T>().FirstOrDefaultAsync(predicate));

        public async Task<T> UpdateAsync<T>(T entity, Action<T> update) where T : class
        {
            update(entity);

            Set<T>().Update(entity);
            await SaveChangesAsync();

            return entity;
        }

        public async Task<T> UpsertAsync<T>(object key, Action<T> factory, Action<T> update) where T : class, new()
            => await UpdateAsync(await GetAsync(key, factory), update);

        public async Task<T> UpdateAsync<T>(Expression<Func<T, bool>> predicate, Action<T> update) where T : class, new()
            => await UpdateAsync(await Set<T>().FirstAsync(predicate), update);

        // User methods
        public Task<User> GetUserAsync(ulong id, ulong guildId)
            => GetAsync<User>(id, x =>
            {
                x.Id = id;
                x.GuildId = guildId;
            });

        public Task<User> GetUserAsync(IGuildUser guildUser)
            => GetUserAsync(guildUser.Id, guildUser.GuildId);

        public Task UpsertUserAsync(ulong id, ulong guildId, Action<User> update)
            => UpsertAsync(id, x =>
            {
                x.Id = id;
                x.GuildId = guildId;
            }, update);

        public Task UpsertUserAsync(IGuildUser guildUser, Action<User> update)
            => UpsertUserAsync(guildUser.Id, guildUser.GuildId, update);

        // Guild methods
        public Task<Guild> GetGuildAsync(ulong id)
            => GetAsync<Guild>(id, x => x.Id = id);

        public Task UpsertGuildAsync(ulong id, Action<Guild> update)
            => UpsertAsync(id, x => x.Id = id, update);
    }
}
