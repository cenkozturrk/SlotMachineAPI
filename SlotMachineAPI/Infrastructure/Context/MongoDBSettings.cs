namespace SlotMachineAPI.Infrastructure.Context
{
    public class MongoDBSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string PlayersCollection { get; set; }
    }
}
