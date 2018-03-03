using FFA.Database;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FFA.Extensions.Database
{
    public static class IMongoCollectionExtensions
    {
        public static async Task<bool> AnyAsync<T>(this IMongoCollection<T> collection, Expression<Func<T, bool>> filter)
            => await collection.CountAsync(filter) != 0;

        public static async Task<IList<T>> FindAsync<T>(this IMongoCollection<T> collection)
        {
            var result = await collection.FindAsync(FilterDefinition<T>.Empty);
            return await result.ToListAsync();
        }

        public static async Task DeleteOneAsync<T>(this IMongoCollection<T> collection, T entity) where T : Entity
            => await collection.DeleteOneAsync(x => x.Id == entity.Id);

        public static async Task<IEnumerable<T>> WhereAsync<T>(this IMongoCollection<T> collection, Expression<Func<T, bool>> filter)
        {
            var result = await collection.FindAsync(filter);
            return result.ToEnumerable();
        }

        public static async Task<T> FindOneAsync<T>(this IMongoCollection<T> collection, Expression<Func<T, bool>> filter) where T : Entity
        {
            var result = await collection.FindAsync(filter);
            var entity = await result.FirstOrDefaultAsync();

            return entity == default(T) ? null : entity;
        }

        public static Task<T> GetAsync<T>(this IMongoCollection<T> collection, Expression<Func<T, bool>> filter, UpdateDefinition<T> factory)
            where T : Entity
            => collection.FindOneAndUpdateAsync(filter, factory, new FindOneAndUpdateOptions<T, T>
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            });

        public static async Task<T> UpdateAsync<T>(this IMongoCollection<T> collection, T entity, Action<T> update) where T : Entity
        {
            update(entity);
            entity.LastModified = DateTimeOffset.UtcNow;

            await collection.ReplaceOneAsync(x => x.Id == entity.Id, entity);

            return entity;
        }

        public static async Task<T> UpsertAsync<T>(this IMongoCollection<T> collection, Expression<Func<T, bool>> filter,
            Action<T> update, UpdateDefinition<T> factory) where T : Entity
            => await collection.UpdateAsync(await collection.GetAsync(filter, factory), update);
    }
}
