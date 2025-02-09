using MongoDB.Driver;
using SlotMachineAPI.Domain.Common;
using SlotMachineAPI.Infrastructure.Context;

namespace SlotMachineAPI.Infrastructure.Repositories
{
    /// <summary>
    /// Generic repository implementation for MongoDB.
    /// Provides common data access operations such as retrieving, adding, updating, and deleting entities.
    /// Designed to be used with any entity that implements the <see cref="IEntity"/> interface.
    /// </summary>
    /// <typeparam name="T">The type of entity being managed by the repository.</typeparam>
    public class Repository<T> : IRepository<T> where T : IEntity
    {
        private readonly IMongoCollection<T> _collection;

        public Repository(MongoDBContext context, string collectionName)
        {
            _collection = context.GetCollection<T>(collectionName);
        }

        public async Task<T> GetByIdAsync(string id) =>
            await _collection.Find(e => e.Id == id).FirstOrDefaultAsync();

        public async Task<List<T>> GetAllAsync() =>
            await _collection.Find(_ => true).ToListAsync();

        public async Task AddAsync(T entity) =>
            await _collection.InsertOneAsync(entity);

        public async Task UpdateAsync(string id, T entity) =>
            await _collection.ReplaceOneAsync(e => e.Id == id, entity);

        public async Task DeleteAsync(string id) =>
            await _collection.DeleteOneAsync(e => e.Id == id);
    }
}
