using SlotMachineAPI.Domain;
using SlotMachineAPI.Domain.Entities;

namespace SlotMachineAPI.Infrastructure.Repositories.Interfaces
{
    public interface ISlotMachineSettingsRepository
    {
        Task<SlotMachineSettings> GetSettingsAsync();
    }
}
