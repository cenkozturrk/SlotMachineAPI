using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SlotMachineAPI.Domain;
using SlotMachineAPI.Infrastructure.Context;

namespace SlotMachineAPI.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(IOptions<MongoDBSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _users = database.GetCollection<User>("Users"); // MongoDB Collaction name
        }

        public async Task<User> GetByEmailAsync(string email) =>
            await _users.Find(user => user.Email == email).FirstOrDefaultAsync();

        public async Task<User> GetByRefreshTokenAsync(string refreshToken) =>
            await _users.Find(user => user.RefreshToken == refreshToken).FirstOrDefaultAsync();

        public async Task AddAsync(User user) => await _users.InsertOneAsync(user);

        public async Task UpdateAsync(string id, User user) =>
            await _users.ReplaceOneAsync(u => u.Id == id, user);
    }
}
