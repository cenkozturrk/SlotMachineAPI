using SlotMachineAPI.Domain;
using SlotMachineAPI.Infrastructure.Context;

namespace SlotMachineAPI.Infrastructure.Repositories
{
    /// <summary>
    /// Repository class for Player entity.
    /// Provides data access methods using MongoDB.
    /// </summary>
    public class PlayerRepository : Repository<Player>, IPlayerRepository
    {
        public PlayerRepository(MongoDBContext context) : base(context, "Players")
        {
        }
    }
}
