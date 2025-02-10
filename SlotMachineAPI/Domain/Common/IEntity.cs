using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace SlotMachineAPI.Domain.Common
{
    public interface IEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        string Id { get; set; }
    }
}
