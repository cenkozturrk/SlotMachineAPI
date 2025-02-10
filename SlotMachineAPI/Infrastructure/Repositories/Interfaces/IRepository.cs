using SlotMachineAPI.Domain.Common;

namespace SlotMachineAPI.Infrastructure.Repositories.Interfaces
{
    public interface IRepository<T> where T : IEntity
    {
        Task<T> GetByIdAsync(string id);
        Task<List<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task UpdateAsync(string id, T entity);
        Task DeleteAsync(string id);
    }
}
