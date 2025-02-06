using SlotMachineAPI.Domain;
using SlotMachineAPI.Infrastructure.Context;

namespace SlotMachineAPI.Infrastructure.Repositories
{
    public class PlayerRepository : Repository<Player>, IPlayerRepository
    {
        public PlayerRepository(MongoDBContext context) : base(context, "Players")
        {
        }
    }
}
