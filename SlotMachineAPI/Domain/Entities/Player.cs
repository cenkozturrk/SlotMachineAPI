using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using SlotMachineAPI.Domain.Common;

namespace SlotMachineAPI.Domain
{
    public class Player : IEntity
    {
        [BsonId] 
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; } = 100.00m;
    }
}
