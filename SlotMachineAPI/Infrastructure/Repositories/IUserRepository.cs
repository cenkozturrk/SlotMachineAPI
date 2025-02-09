using SlotMachineAPI.Domain;

namespace SlotMachineAPI.Infrastructure.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByRefreshTokenAsync(string refreshToken);
        Task AddAsync(User user);
        Task UpdateAsync(string id, User user);
    }
}
