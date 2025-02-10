using MongoDB.Driver;
using SlotMachineAPI.Domain.Entities;
using SlotMachineAPI.Infrastructure.Context;
using SlotMachineAPI.Infrastructure.Repositories.Interfaces;

namespace SlotMachineAPI.Infrastructure.Repositories.Implementations
{
    public class SlotMachineSettingsRepository : ISlotMachineSettingsRepository
    {
        private readonly IMongoCollection<SlotMachineSettings> _settingsCollection;
        public SlotMachineSettingsRepository(MongoDBContext context)
        {
            _settingsCollection = context.GetCollection<SlotMachineSettings>("SlotMachineSettings");
        }

        public async Task<SlotMachineSettings> GetSettingsAsync()
        {
            return await _settingsCollection.Find(_ => true).FirstOrDefaultAsync();
        }
    }
}
